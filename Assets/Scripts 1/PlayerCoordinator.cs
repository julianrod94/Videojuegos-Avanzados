using UnityEngine;

public class PlayerCoordinator : MonoBehaviour {
    
    private static PlayerCoordinator instance;
    
    public GameObject ball;
    public GameObject weapon;
    public GameObject holder;
    public GameObject player;

    public ParticleSystem blood;

    public int life = 3;

    public bool holding = false;
    
    private PlayerCoordinator() {
    }

    public static PlayerCoordinator Instance {
        get { return instance; }
    }

    public void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void shoot(float weaponRange, Transform origin) {
        if (holding) {
            dropBall();
        }
        
        Vector3 fwd = origin.transform.TransformDirection(Vector3.forward);

        Debug.DrawRay(origin.transform.position, fwd, Color.green,2);

        RaycastHit hit;
        AudioManager.Instance.playerShoot();
        
        //TODO SHOOT AND MISS OR SHOOT AND NOT MISS
        if (Physics.Raycast(origin.transform.position, fwd, out hit, weaponRange)) {
            GameObject obj = hit.transform.gameObject;
            Debug.Log(obj.name);
            switch (obj.tag) {
                case Tags.enemy:
                    obj.GetComponentInParent<EnemyLife>().damage(obj.GetComponent<EnemyCollider>().damage);
                    Instantiate(blood, hit.point, Quaternion.identity);
                    AudioManager.Instance.enemyHit(obj.GetComponentInParent<AudioSource>());
                    break;
            }
        }
    }

    public void grabBall() {
        if(holding) return;
        
        holding = true;
        
        // Set ball as child
        ball.gameObject.transform.parent = holder.gameObject.transform;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        ball.GetComponent<Collider>().enabled = false;
        ball.transform.position = holder.transform.position;
        
        //Hide weapon
        weapon.gameObject.SetActive(false);   
    }
    
    
    public void dropBall() {
        if(!holding) return;

        holding = false;
        
        // Leave ball on the floor
        ball.gameObject.transform.parent = null;
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.GetComponent<Rigidbody>().AddForce(Vector3.right * 5, ForceMode.Impulse);
        ball.GetComponent<Collider>().enabled = true;
        
        //Show weapon
        weapon.gameObject.SetActive(true);
    }

    public void dealDamage() {
        life --;
        Destroy(GameObject.Find("hearth" + life));
        AudioManager.Instance.playerHit();
        if (life <= 0) {
            ScenesOrganizer.Instance.goToEndGame();
        }

    }
    
}