using UnityEngine;

public class bulletScript : MonoBehaviour
{

    [SerializeField] int bulletspeed;
    [SerializeField] int bullettime;

    void Start()
    {
        Destroy(gameObject, bullettime);
    }


    void Update()
    {
        transform.Translate(0, 0, 1 * Time.deltaTime * bulletspeed, 0);
    }
}
