using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public Crosshairs crosshairs;
    public float moveSpeed = 1.8f;
    PlayerController controller;
    GunController gunController;
    Animator anim;
    bool runKeyDown;

    Camera viewCamera; // 마우스 포인터 생성을 위한 카메라 오브젝트 생성

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main; //viewCamera에 메인카메라 지정
        gunController = GetComponent<GunController>();
        moveSpeed = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //이동 제어
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed * (runKeyDown ? 1.8f : 1f);
        controller.Move(moveVelocity);

        //시선처리 제어
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition); //카메라로부터 스크린으로 이어지는 ray 발사
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance) + new Vector3(0, 1, 0);
            Debug.DrawLine(ray.origin, point, Color.red); //Debug로실제 붉은 선 확인
            controller.LookAt(point);
            crosshairs.transform.transform.position = point;
            crosshairs.DetectTargets(ray);
        }

        //달리기 모션 제어
        runKeyDown = Input.GetKey(KeyCode.LeftShift);
        anim.SetBool("isWalkFront", moveInput != Vector3.zero);
        anim.SetBool("isRun", runKeyDown);
        anim.SetBool("isShoot", Input.GetMouseButton(0));

        //무기발사 제어
        if (Input.GetMouseButton(0) && runKeyDown != true)
        {
            gunController.Shoot();
        }
    }
    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
        Cursor.visible = true;
    }
}
