using System.Collections.Generic;
using UnityEngine;

/* -----FORMULAS-----
 * Direction: (endPos-startPos)
 * Velocity: (endPos-startPos) * valueF
 * Bullet trajectory: position + velocity * time + 0.5f * gravity * (time)^2
 */

public class Weapon : MonoBehaviour
{
    [SerializeField] bool isFiring = false;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Transform raycastDestination;
    [SerializeField] TrailRenderer tracerEffect;
    [SerializeField] ParticleSystem hitEffect;

    //The higher the faster
    [SerializeField] int fireRate = 25;

    //The float can be changed to simulate real bullet speeds;
    [SerializeField] float bulletSpeed = 1000f;

    [SerializeField] float bulletDrop = 0.0f;

    List<Bullet> bulletList = new List<Bullet>();

    float bulletMaxLifeTime = 3f;
    float accumulatedTime;
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

    private void LateUpdate()
    {
        //Separate inputs in actual game
        if (Input.GetButtonDown("Fire1"))
        {
            StartFiring();
        }

        if (isFiring)
        {
            FireInterval(Time.deltaTime);
        }

        UpdateBullets(Time.deltaTime);

        if (Input.GetButtonUp("Fire1"))
        {
            StopFiring();
        }
    }

    void StartFiring()
    {
        isFiring = true;

        FireBullet();
    }

    void FireBullet()
    {
        //FORMULA
        //(endPos-startPos) = direction /  = velocity;
        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;

        //Create and grab the bullet instance
        Bullet bullet = CreateBullet(raycastOrigin.position, velocity);

        //Add the bullet instance to the bullet list
        bulletList.Add(bullet);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet tempBullet = new Bullet();

        //Initialize the bullet parameters
        tempBullet.initialPosition = position;
        tempBullet.initialVelocity = velocity;
        tempBullet.time = 0.0f;

        //Create the bullet trace effect
        tempBullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        tempBullet.tracer.AddPosition(position);

        //Get the bullet after its created
        return tempBullet;
    }

    void FireInterval(float deltaTime)
    {
        accumulatedTime += deltaTime;

        float fireInterval = 1.0f / fireRate;

        while (accumulatedTime >= 0f)
        {
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }

    void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    void SimulateBullets(float deltaTime)
    {
        bulletList.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;

            Vector3 p1 = GetPosition(bullet);

            RaycastSegment(p0, p1, bullet);
        });
    }

    Vector3 GetPosition(Bullet bullet)
    {
        //position + velocity * time + 0.5f * gravity * (time)^2
        Vector3 gravity = Vector3.down * bulletDrop;

        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        ray.origin = start;
        ray.direction = end - start;

        float distance = (end - start).magnitude;

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;

            hitEffect.Emit(1);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = bulletMaxLifeTime;
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    void DestroyBullets()
    {
        bulletList.RemoveAll(bullet => bullet.time >= bulletMaxLifeTime);
    }

    void StopFiring()
    {
        isFiring = false;
    }
}    
