using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using YmirEngine;

public class QueenXenomorphSpitAttack : YmirComponent
{
    public GameObject thisReference = null;

    private float movementSpeed;

    private float damage = 600f;

    private GameObject player;

    private Health healthScript;

    private bool destroyed;

    private float destroyTimer;

    public void Start()
    {
        movementSpeed = 7000f;
        player = InternalCalls.GetGameObjectByName("Player");
        healthScript = player.GetComponent<Health>();
        Vector3 impulse = gameObject.transform.GetForward();
        impulse += new Vector3(0, -0.07f, 0);
        gameObject.SetImpulse(impulse * movementSpeed * Time.deltaTime);
        destroyed = false;
        destroyTimer = 0f;
    }

    public void Update()
    {
        destroyTimer += Time.deltaTime;

        if (destroyed || destroyTimer >= 3f)
        {
            InternalCalls.Destroy(gameObject);
        }

    }

    public void OnCollisionStay(GameObject other)
    {
        if (other.Name == "Player" && destroyed == false)
        {
            healthScript.TakeDmg(damage);
            destroyed = true;
        }
    }

}