using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviourPun, IPunObservable
{

    public int MAX_HP = 100;
    public int HP = 0;
    public int DAMAGE = 2;
    public float speed;
    public float v_speed_coef;
    public Text Nickname;
    public Text Message;
    public Transform[] attackPoints;
    public GameObject[] attackAnimations;
    public float range = 1f;
    public LayerMask enemyLayers;
    public ParticleSystem on_hit;
    public Image playerHP;
    private PhotonView photonView;
    private Rigidbody2D rb;
    private Camera myCamera;
    private Animator animator;
    private InputField SendingMessage;
    int attack_cooldown = 0;
    int attack_frequency = 90;
    int respawn_cooldown = 0;
    int respawn_frequency = 300;

    bool Is_right = true;
    bool Got_hit = false;
    bool Is_dead = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if (stream.IsWriting)
        {
            stream.SendNext(Got_hit);
        }
        else 
        {
            Got_hit = (bool) stream.ReceiveNext();
        }
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        HP = MAX_HP;

        Nickname.text = photonView.Owner.NickName;

        if (photonView.Owner.IsLocal)
            Camera.main.GetComponent<CameraFollow>().player = gameObject.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (photonView.IsMine && !Is_dead)
        {
            float moveHorizontal = Input.GetAxis ("Horizontal");
            float moveVertical = Input.GetAxis ("Vertical");

            float sqr_speed = moveHorizontal*moveHorizontal + moveVertical*moveVertical;

            animator.SetFloat("Speed", sqr_speed);

            if (moveHorizontal < -0.1){
                animator.SetBool("Is_right", false);
                Is_right = false;
            }
            else if (moveHorizontal > 0.1){
                animator.SetBool("Is_right", true);
                Is_right = true;
            }

            if (Input.GetMouseButtonDown(0) & attack_cooldown == 0){
                Attack();
            }

            Vector3 movement = new Vector3 (moveHorizontal, moveVertical * v_speed_coef, 0f);
            transform.Translate(movement * speed * Time.fixedDeltaTime);
        }

        if (Got_hit){
            on_hit.Play();
        }
        attack_cooldown = ChangeCooldown(attack_cooldown, -1);
        UpdateHPBar();
        if (HP <= 0){
            if (!Is_dead) {
                Death();
            }
            else {
                respawn_cooldown = ChangeCooldown(respawn_cooldown, 1);
                if (respawn_cooldown >= respawn_frequency) {
                    Respawn();
                }
            }
        }
    }


    void Attack(){
        attack_cooldown = ChangeCooldown(attack_cooldown, attack_frequency);
        animator.SetTrigger("Attack");
        if (!Is_right){
            Collider2D[] enemys_hit = Physics2D.OverlapCircleAll(attackPoints[0].position, range, enemyLayers);
            foreach (Collider2D enemy in enemys_hit){
                enemy.GetComponent<PhotonView>().RPC("GetHit", RpcTarget.AllBuffered, DAMAGE);
            }
        }
        else if (Is_right){
            Collider2D[] enemys_hit = Physics2D.OverlapCircleAll(attackPoints[1].position, range, enemyLayers);
            foreach (Collider2D enemy in enemys_hit){
                enemy.GetComponent<PhotonView>().RPC("GetHit", RpcTarget.AllBuffered, DAMAGE);
            }
        }
    }

    [PunRPC]
    public void GetHit(int taken_damage){
        on_hit.Play();
        HP -= DAMAGE;
        if (HP < 0) HP = 0;
        playerHP.fillAmount = HP / MAX_HP;
    }

    [PunRPC]
    public void punSetMessage(string message){
        Message.text = message;
    }

    public void setMessage(){
        photonView.RPC("punSetMessage", RpcTarget.AllBuffered, SendingMessage.text);
    }

    public void Death(){
        Is_dead = true;
        animator.SetTrigger("Death");
        animator.SetBool("Is_dead", Is_dead);
    }

    public void Respawn(){
        respawn_cooldown = 0;
        Is_dead = false;
        animator.SetBool("Is_dead", Is_dead);
        HP = MAX_HP;
    }

    void UpdateHPBar(){
        playerHP.fillAmount = (float) HP / MAX_HP;
    }

    void UpdateChatBox(){
        setMessage();
    }

    int ChangeCooldown(int cooldown, int num){
        cooldown += num;
        if (cooldown < 0) cooldown = 0;
        return cooldown;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoints[0].position, range);
        Gizmos.DrawWireSphere(attackPoints[1].position, range);
    }
}
