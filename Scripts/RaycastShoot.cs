using UnityEngine;

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
            DamageSender damageSender = new DamageSender(totalDamagePerDamageable[i], gameObject, DamageType.Projectile);
            totalDamageables[i].Damage(damageSender);
        }
    }
}
