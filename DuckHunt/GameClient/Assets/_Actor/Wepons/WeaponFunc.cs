using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFunc : MonoBehaviour
{
    public bool shotenabled = true;

    public ParticleSystem muzzleFlash;
    public ParticleSystem bulletTrail;
    public GameObject bulletHit;
    public GameObject playerHit;
    public Animation recoil;
    public Animation reloadA;
    public Camera aimcamera;
    bool trigger = false;
    bool empty = false;
    bool reload = false;
    bool reloadLock = false;

    public int shotid;

    Vector3 direction;
    Vector3 lookpos;

    RaycastHit hitresult;

    public AudioClip shot;
    public AudioClip emptyAudio;
    public int rpm = 450;
    public int magazine = 20;
    public int damage = 5;
    public float reloadTime = 1.0F;

    int rounds;
    float reload_timer;
    float gun_range;
    float gun_timer;
    float gunShot_delay;

    public int getRounds()
    {
        return rounds;
    }

    void Start()
    {
        //Convert RPM to shotdelay
        {
            float e = rpm / 60;
            e = 1 / e;
            gunShot_delay = e;
        }

        aimcamera = PlayerController.instance.playercamera;

        rounds = magazine;
        reload_timer = reloadTime;
        gun_range = 50.0F;
        gun_timer = gunShot_delay + 1F;
    }

    void Update()
    {
        if (shotenabled == false)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0) && gun_timer >= gunShot_delay)
        {
            trigger = true;
            gun_timer = 0F;
        }
        if(Input.GetKey(KeyCode.R) && rounds != magazine && reload_timer >= reloadTime)
        {
            reload = true;
            //reloadA.Play();
            reload_timer = 0F;
        }
    }

    void FixedUpdate()
    {
        if (shotenabled == false)
        {
            return;
        }

        //lookpos = transform.position;
        lookpos = aimcamera.transform.position;
        //direction = transform.forward;
        direction = aimcamera.transform.forward;

        if (reload)
        {
            reload = false;
            reloadLock = true;

            StartCoroutine(canShootAgain(reloadTime));
            
            reloadA.Play();
            rounds = magazine;

            HUD.instance.setAmmo(rounds);
            return;
        }

        if (trigger && !reloadLock)
        {
            trigger = false;
            if(rounds <= 0)
            {
                if (!empty)
                {
                    this.GetComponent<AudioSource>().PlayOneShot(emptyAudio, GameInstance.instance.MasterVolume);
                    empty = true;
                }
                return;
            }
            else
            {
                empty = false;
            }

            rounds -= 1;
            HUD.instance.setAmmo(rounds);
            muzzleFlash.Play();

            this.GetComponent<AudioSource>().PlayOneShot(shot, GameInstance.instance.MasterVolume);
            recoil.Stop();
            recoil.Play();

            ClientSend.playershot(Client.instance.myId, shotid, lookpos, gameObject.transform.rotation);

            if (Physics.Raycast(lookpos, direction, out hitresult, gun_range))
            {
                bulletTrail.transform.LookAt(hitresult.point);
                if (Gamemode.instance.playoffline)
                {
                    OFFHealth h = hitresult.collider.gameObject.GetComponentInParent<OFFHealth>();
                    if (h != null)
                    {
                        h.getDamage(damage);
                        Instantiate(playerHit, hitresult.point, Quaternion.LookRotation(hitresult.normal));
                    }
                    else
                    {
                        Instantiate(bulletHit, hitresult.point, Quaternion.LookRotation(hitresult.normal));
                    }
                }
                else
                {
                    Health h = hitresult.collider.gameObject.GetComponentInParent<Health>();
                    if (h != null)
                    {
                        h.Damage(damage);
                        Instantiate(playerHit, hitresult.point, Quaternion.LookRotation(hitresult.normal));
                    }
                    else
                    {
                        Instantiate(bulletHit, hitresult.point, Quaternion.LookRotation(hitresult.normal));
                    }
                }
                
            }
            else
            {
                bulletTrail.transform.localRotation = new Quaternion(0,0,0,0);
            }

            bulletTrail.Play();
        }

        gun_timer += Time.deltaTime;
        reload_timer += Time.deltaTime;
    }

    /*void OnDrawGizmos()
    {
        if (trigger)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(lookpos, direction * hitresult.distance);
        }
        else if (!trigger)
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawRay(lookpos, direction * gun_range);
        }
    }*/

    IEnumerator canShootAgain(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        reloadLock = false;
    }
}
