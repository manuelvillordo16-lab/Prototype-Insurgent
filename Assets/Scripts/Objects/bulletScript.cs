using UnityEngine;

public class bulletScript : MonoBehaviour
{

    [SerializeField] float bulletspeed = 20;
    [SerializeField] int bullettime;

    void Start()
    {
        Destroy(gameObject, bullettime);
    }


    void Update()
    {
        transform.Translate(Vector3.forward * bulletspeed * Time.deltaTime);
    }
}
