using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 5;

    private void Start()
    {
        lifetime -= Time.deltaTime;

        if (lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    //i believe this part of the script does not work. Please refer to the OnTriggerEnter
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("i shot the player");
            collision.gameObject.GetComponent<Movement>().health -= damage;
        }
        Destroy(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            other.GetComponent<Movement>().health -= damage;
        }

        Destroy(gameObject);
    }
}
