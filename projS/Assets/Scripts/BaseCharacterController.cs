﻿using UnityEngine;
using System.Collections;

public class BaseCharacterController : MonoBehaviour {

	public Vector2 velocityMin = new Vector2(-100.0f,-100.0f);
	public Vector2 velocityMax = new Vector2(+100.0f,+50.0f);

	[System.NonSerialized] public float hpMax = 10.0f;
	[System.NonSerialized] public float hp = 10.0f;
	[System.NonSerialized] public float dir = 1.0f;
	[System.NonSerialized] public float speed = 6.0f;
	[System.NonSerialized] public float basScaleX = 1.0f;
	[System.NonSerialized] public bool activeSts = false;
	[System.NonSerialized] public bool jumped = false;
	[System.NonSerialized] public bool grounded = false;
	[System.NonSerialized] public bool groundedPrev = false;

	[System.NonSerialized] public Animator animator;
	protected Transform groundCheck_L;
	protected Transform groundCheck_C;
	protected Transform groundCheck_R;

	protected float speedVx = 0.0f;
	protected float speedVxAddPower = 0.0f;
	protected float gravityScale = 10.0f;
	protected float jumpStartTime = 0.0f;

	protected GameObject groundCheck_OnRoadObject;
	protected GameObject groundCheck_OnMoveObject;
	protected GameObject groundCheck_OnEnemyObject;

	protected bool addForceVxEnabled = false;
	protected float addForceVxStartTime = 0.0f;

	protected bool addVelocityEnabled = false;
	protected float addVelocityVx = 0.0f;
	protected float addVelocityVy = 0.0f;

	protected bool setVelocityVxEnabled = false;
	protected bool setVelocityVyEnabled = false;
	protected float setVelocityVx = 0.0f;
	protected float setVelocityVy = 0.0f;

	public bool superAmor = false;
	public bool superAmor_jumpAttackDmg = true;

	public GameObject[] fireObjectList;

	protected virtual void Awake(){
		animator = GetComponentInChildren<Animator> ();
		groundCheck_L = transform.Find ("GroundCheck_L");
		groundCheck_C = transform.Find ("GroundCheck_C");
		groundCheck_R = transform.Find ("GroundCheck_R");

		dir = (transform.localScale.x > 0.0f) ? 1 : -1;
		basScaleX = transform.localScale.x * dir;
		transform.localScale = new Vector3 (basScaleX, transform.localScale.y, transform.localScale.z);
		activeSts = true;
		gravityScale = rigidbody2D.gravityScale;
	}

	// Use this for initialization
	protected virtual void Start () {
	
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	
	}

	protected virtual void FixedUpdate(){
		if (transform.position.y < -30.0f) {
			Dead(false);
		}

		groundedPrev = grounded;
		grounded = false;

		groundCheck_OnRoadObject = null;
		groundCheck_OnMoveObject = null;
		groundCheck_OnEnemyObject = null;

		Collider2D[][] groundCheckCollider = new Collider2D[3][];
		groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position);
		groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position);
		groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);

		foreach (Collider2D[] groundCheckList in groundCheckCollider) {
			foreach(Collider2D groundCheck in groundCheckList){
				if(groundCheck !=null){
					if(!groundCheck.isTrigger){
						grounded = true;
						if(groundCheck.tag=="Road"){
							groundCheck_OnRoadObject = groundCheck.gameObject;
						}else if(groundCheck.tag == "MoveObject"){
							groundCheck_OnMoveObject = groundCheck.gameObject;
						}else if(groundCheck.tag == "Enemy"){
							groundCheck_OnEnemyObject = groundCheck.gameObject;
						}
					}
				}
			}
		}

		FixedUpdateCharacter ();

		if(grounded){
			speedVxAddPower = 0.0f;
			if(groundCheck_OnMoveObject !=null){
				Rigidbody2D rb2D = groundCheck_OnMoveObject.GetComponentInParent<Rigidbody2D>();
				speedVxAddPower = rb2D.velocity.x;
			}
		}


		if (addForceVxEnabled) {
			if(Time.fixedTime - addForceVxStartTime > 0.5f){
				addForceVxEnabled = false;
			}
		}else{
			rigidbody2D.velocity = new Vector2 (speedVx+speedVxAddPower, rigidbody2D.velocity.y);
		}

		if (addVelocityEnabled) {
			addVelocityEnabled = false;
			rigidbody2D.velocity = new Vector2(
				rigidbody2D.velocity.x + addVelocityVx,
				rigidbody2D.velocity.y + addVelocityVy);
		}

		if(setVelocityVxEnabled){
			setVelocityVxEnabled = false;
			rigidbody2D.velocity = new Vector2(setVelocityVx,rigidbody2D.velocity.y);
		}
		if(setVelocityVyEnabled){
			setVelocityVyEnabled = false;
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,setVelocityVy);
		}


		float vx = Mathf.Clamp (rigidbody2D.velocity.x, velocityMin.x, velocityMax.x);
		float vy = Mathf.Clamp (rigidbody2D.velocity.y, velocityMin.y, velocityMax.y);
		rigidbody2D.velocity = new Vector2 (vx, vy);
	}

	protected virtual void FixedUpdateCharacter(){
	}

	public virtual void AddForceAnimatorVx(float vx){
		if(vx != 0.0f){
			rigidbody2D.AddForce(new Vector2(vx*dir,0.0f));
			addForceVxEnabled = true;
			addForceVxStartTime = Time.fixedTime;
		}
	}

	public virtual void AddForceAnimatorVy(float vy){
		if(vy != 0.0f){
			rigidbody2D.AddForce(new Vector2(0.0f,vy));
			jumped =true;
			jumpStartTime = Time.fixedTime;
		}
	}

	public virtual void AddVelocityVx(float vx){
		addVelocityEnabled = true;
		addVelocityVx = vx * dir;
	}

	public virtual void AddVelocityVy(float vy){
		addVelocityEnabled = true;
		addVelocityVy = vy;
	}

	public virtual void SetVelocityVx(float vx){
		setVelocityVxEnabled = true;
		setVelocityVx = vx * dir;
	}

	public virtual void SetVelocityVy(float vy){
		setVelocityVyEnabled = true;
		setVelocityVy = vy;
	}

	public virtual void SetLightGravity(){
		rigidbody2D.velocity = new Vector2 (0.0f, 0.0f);
		rigidbody2D.gravityScale = 0.1f;
	}

	public virtual void ActionMove(float n){
		if (n != 0.0f) {
			dir = Mathf.Sign (n);
			speedVx = speed * n;
			animator.SetTrigger ("Run");
		} else {
			speedVx = 0;
			animator.SetTrigger("Idle");
		}
	}

	public void ActionFire(){
		Transform goFire = transform.Find ("Muzzle");
		foreach(GameObject fireObject in fireObjectList){
			GameObject go = Instantiate(fireObject,goFire.position,Quaternion.identity) as GameObject;
			go.GetComponent<FireBullet>().ownwer = transform;
		}
	}

	public virtual void Dead(bool gameOver){
		if (!activeSts) {
			return;
		}
		activeSts = false;
		animator.SetTrigger ("Dead");
	}

	public virtual bool SetHP(float _hp,float _hpMax){
		hp = _hp;
		hpMax = _hpMax;
		return(hp <= 0);
	}

	public void EnableSuperAmor(){
		superAmor = true;
	}
	public void DisableSuperAmor(){
		superAmor = false;
	}

	public bool ActionLookUp(GameObject go,float near){
		if(Vector3.Distance(transform.position,go.transform.position)>near){
			dir= (transform.position.x < go.transform.position.x)?+1:-1;
			return true;
		}
		return false;
	}

	public bool ActionMoveToNear(GameObject go,float near){
		if(Vector3.Distance(transform.position,go.transform.position)>near){
			ActionMove ((transform.position.x < go.transform.position.x)?+1.0f:-1.0f);
			return true;
		}
		return false;
	}

	public bool ActionMoveToFar(GameObject go,float far){
		if(Vector3.Distance(transform.position,go.transform.position)<far){
			ActionMove ((transform.position.x>go.transform.position.x)?+1.0f:-1.0f);
			return true;
		}
		return false;
	}
}