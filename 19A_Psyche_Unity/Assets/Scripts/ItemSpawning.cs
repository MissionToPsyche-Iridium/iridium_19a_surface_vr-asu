using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSpawning : MonoBehaviour
{
    
    [SerializeField] private List<GameObject> items = new List<GameObject>();
    [Range(0,1)] [SerializeField] private float density = 0;
    [SerializeField] private float[] xRange = {0,1};
    [SerializeField] private float[] zRange = {0,1};
    [SerializeField] private float yValue = 1;
    [SerializeField] private float ySpawnOffset = 0;

    [SerializeField] private float maxRayDist = 1000;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(xRange[0], yValue, zRange[0]);
        HandleSpawning();
        Destroy(gameObject);
    }

    private void HandleSpawning()
    {
        while(transform.position.z > zRange[1])
        {
            foreach (var item in items)
            {
                while(transform.position.x < xRange[1])
                {
                    if(Random.Range(0f,1f) < density || density == 1f)
                    {
                        SpawnItem(item);
                    }
                    transform.Translate(Vector3.right * Random.Range(10, 100));
                }
                transform.position = new Vector3(xRange[0], transform.position.y, transform.position.z);
            }
            transform.Translate(Vector3.forward * Random.Range(10, 100) * -1);
        }
        
    }

    private void SpawnItem(GameObject item)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, maxRayDist))
        {
            if(hit.transform.tag.Equals("Terrain"))
            {
                Instantiate(item, hit.point + Vector3.up * ySpawnOffset, Quaternion.identity);
            }
        }
    }

}
