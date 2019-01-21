using UnityEngine;
using System.Collections;

public class ObstacleBase : MonoBehaviour {
    public GameObject fragment; // 파편의 프리팹

    private bool bDestructable; // 파괴 가능 여부
    private bool bFragments; // 파편 생성 여부

    
	// Use this for initialization
	void Start () {
        bFragments = true;
        bDestructable = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "SampleBullet")
        {
            Destroy(other);

            if (bDestructable)
            {
                Destroy(gameObject);

                // 파편이 있을 경우
                if (bFragments)
                {
                    GameObject IFragment;
                    IFragment = (GameObject)(MonoBehaviour.Instantiate(fragment, transform.position, transform.rotation));

                    // 1.8초 후에 파편이 사라짐
                    Destroy(IFragment, 1.8f);
                }
            }
        }
    }
}
