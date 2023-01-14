using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSoundPlayer : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextureSound[] textureSounds;
    private CharacterController controller;
    private AudioSource audioSource;

    [SerializeField] private bool blendTerrainSounds;
    [SerializeField] private float footStepDelay = 0.5f;
    private WaitForSeconds _footStepDelay;

    private void Awake()
    {
        controller = playerController.characterController;
        audioSource = GetComponent<AudioSource>();
        _footStepDelay = new WaitForSeconds(footStepDelay);
    }

    private void Start()
    {
        StartCoroutine(CheckGround());
    }

    private IEnumerator CheckGround()
    {
        while(true)
        {
            if(playerController.grounded && controller.velocity != Vector3.zero && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, playerController.groundMask))
            {
                if(hit.collider.TryGetComponent(out Terrain terrain))
                {
                    yield return StartCoroutine(PlayFootstepSoundFromTerrain(terrain, hit.point));
                }
                else if(hit.collider.TryGetComponent(out Renderer renderer))
                {
                    yield return StartCoroutine(PlayFootstepSoundFromRenderer(renderer));
                }
            }
            yield return null;
        }
    }

    private IEnumerator PlayFootstepSoundFromRenderer(Renderer renderer)
    {
        foreach (TextureSound textureSound in textureSounds)
        {
            if(textureSound.Albedo == renderer.material.GetTexture("_MainTex"))
            {
                AudioClip clip = GetClipFromTextureSound(textureSound);
                audioSource.PlayOneShot(clip, Random.Range(0.75f, 1f));
                yield return _footStepDelay;
                break;
            }
        }
    }

    private IEnumerator PlayFootstepSoundFromTerrain(Terrain terrain, Vector3 hitPoint)
    {
        Vector3 terrainPosition = hitPoint - terrain.transform.position;
        Vector3 splatMapPosition = new Vector3(terrainPosition.x / terrain.terrainData.size.x, 0, terrainPosition.z / terrain.terrainData.size.z);

        int x = Mathf.FloorToInt(splatMapPosition.x * terrain.terrainData.alphamapWidth);
        int z = Mathf.FloorToInt(splatMapPosition.z * terrain.terrainData.alphamapHeight);

        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, z, 1, 1);

        if (!blendTerrainSounds)
        {
            int primaryIndex = 0;
            for (int i = 1; i < alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > alphaMap[0, 0, primaryIndex])
                {
                    primaryIndex = i;
                }
            }

            foreach (TextureSound textureSound in textureSounds)
            {
                if (textureSound.Albedo == terrain.terrainData.terrainLayers[primaryIndex].diffuseTexture)
                {
                    AudioClip clip = GetClipFromTextureSound(textureSound);
                    audioSource.PlayOneShot(clip, Random.Range(0.85f,1f));
                    yield return _footStepDelay;
                    break;
                }
            }
        }
        else
        {
            List<AudioClip> clips = new List<AudioClip>();
            int clipIndex = 0;
            for (int i = 0; i < alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > 0)
                {
                    foreach (TextureSound textureSound in textureSounds)
                    {
                        if (textureSound.Albedo == terrain.terrainData.terrainLayers[i].diffuseTexture)
                        {
                            AudioClip clip = GetClipFromTextureSound(textureSound);
                            audioSource.PlayOneShot(clip, alphaMap[0, 0, i]);
                            clips.Add(clip);
                            clipIndex++;
                            break;
                        }
                    }
                }
            }

            float longestClip = clips.Max(clip => clip.length);
            yield return _footStepDelay;
        }
    }

    private AudioClip GetClipFromTextureSound(TextureSound textureSound)
    {
        int clipIndex = Random.Range(0, textureSound.clips.Length);
        return textureSound.clips[clipIndex];
    }

    [System.Serializable]
    private class TextureSound
    {
        public Texture Albedo;
        public AudioClip[] clips;
    }
}
