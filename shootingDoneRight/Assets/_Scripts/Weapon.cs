using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] bool isFiring = false;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Transform raycastDestination;
    [SerializeField] TrailRenderer tracerEffect;
    [SerializeField] ParticleSystem hitEffect;

    Ray ray;
    RaycastHit hitInfo;

    private void Awake()
    {
        SetDefaults();
    }

    void SetDefaults()
    {
        isFiring = false;
    }

    void StartFiring()
    {
        isFiring = true;

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        TrailRenderer tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if(Physics.Raycast(ray, out hitInfo))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;

            hitEffect.Emit(1);

            tracer.transform.position = hitInfo.point;
        }
    }

    void StopFiring()
    {
        isFiring = false;
    }

    private void LateUpdate()
    {
        //Separate inputs in actual game
        if(Input.GetButtonDown("Fire1"))
        {
            StartFiring();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopFiring();
        }
    }
}
