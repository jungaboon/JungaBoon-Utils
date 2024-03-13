using UnityEngine;

///<summary>
/// This script can be used as a generic way of dealing raycast damage from a weapon
/// It will take into account damage falloff over distance which you can define with an Animation Curve
/// Note that the damage curve should go from 0 to 1 from left to right for the damage calculation to work properly
/// You will need to create an IDamageable interface and a DamageSender struct or replace them with your own
/// IDamageable will need to have a public Damage(DamageSender damageSender) function
/// The raycasts will deal damage based on distance, and it will also separate damage based on what targets it hits
/// This way, if a scattershot hits multiple targets, it won't deal the same damage to all of them, but rather only based on the rays that hit it
///</summary>

public class RaycastShoot : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private int numberOfShots;
    [SerializeField] private float spread;
    [SerializeField] private float maxRange;
    [SerializeField] private AnimationCurve damageCurve;

    protected void Fire()
    {    
        List<IDamageable> totalDamageables = new List<IDamageable>();
        float[] totalDamagePerDamageable = new float[numberOfShots];

        for (int i = 0; i < numberOfShots; i++)
        {
            Vector3 adjustedSpread = new Vector3(Random.Range(-spread,spread), Random.Range(-spread,spread), Random.Range(-spread,spread));
            Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward + adjustedSpread);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, maxRange))
            {
                float distanceDiff = maxRange - hitInfo.distance;
                float distanceRatio = distanceDiff/maxRange;
                float calculatedDamage = damageCurve.Evaluate(distanceRatio) * damageAmount;
                // IDamageable is an Interface you can create or replace with whatever damage script you're using
                if(hitInfo.transform.TryGetComponent(out IDamageable damageable))
                {
                    if(!totalDamageables.Contains(damageable)) totalDamageables.Add(damageable);
                    totalDamagePerDamageable[totalDamageables.IndexOf(damageable)] += calculatedDamage;
                    Debug.DrawRay(hitInfo.point, hitInfo.normal * 2f, Color.red, 3f);
                }
            }
        }

        for (int i = 0; i < totalDamageables.Count; i++)
        {
            // DamageSender is a struct that I used to consolidate the damage info, though you can just use function parameters instead
            DamageSender damageSender = new DamageSender(totalDamagePerDamageable[i], gameObject, DamageType.Projectile);
            totalDamageables[i].Damage(damageSender);
        }
    }
}
