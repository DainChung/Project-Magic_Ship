using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** Ultimate skill script for player
 * @usage add this script to ultimate skill collider prefab
 * @effect reduce targets' movement speed & attack rate
 */
public class UltimateSkill : MonoBehaviour {
    /** Enemy is slowed if collide with this collider */
    protected SphereCollider Collider;

    /** Ultimate skill duration */
    public float SkillDuration;

    /** Slowing effect remains even if enemy escape from area for 'RemainingTime' */
    public float RemainingTime;

    public float MovementSpeedModifier;
    public float RotationSpeedModifier;

    /** List that contains objects in sphere collision to restore their stat after collision is disabled */
    List<Collider> ObjectsInCollision;

	// Use this for initialization
	void Start () {
        Collider = GetComponent<SphereCollider>();

        ObjectsInCollision = new List<Collider>();

        // set default value if initial value is not
        if (Collider.radius <= 0)
        {
            Collider.radius = 20.0f;
        }
        if (SkillDuration <= 0) SkillDuration = 10.0f;
        if (RemainingTime <= 0) RemainingTime = 3.0f;
        if (MovementSpeedModifier <= 0) MovementSpeedModifier = 0.6f;
        if (RotationSpeedModifier <= 0) RotationSpeedModifier = 0.6f;

        StartCoroutine(Timer());
	}

    // Visualize ultimate skill area in editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.5f, 0.6f, 0.4f);
        Gizmos.DrawSphere(transform.position, GetComponent<SphereCollider>().radius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "SampleEnemy")
        {
            Unit__Base_Stat TargetStat = other.GetComponent<UnitBaseEngine>()._unit_Stat;

            // apply skill effect
            TargetStat.__PUB_Move_Speed *= MovementSpeedModifier;
            TargetStat.__PUB_Rotation_Speed *= RotationSpeedModifier;
            
            ObjectsInCollision.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "SampleEnemy")
        {
            StartCoroutine(RemainedSkillEffect(other));

            for (int i = 0; i < ObjectsInCollision.Count; i++)
                if (other.name == ObjectsInCollision[i].name)
                    ObjectsInCollision.RemoveAt(i);
        }
    }

    /** skill effect remains in enemy even if it escaped from ultimate skill area
     * @param other target escaping from ultimate skill
     */
    private IEnumerator RemainedSkillEffect(Collider other)
    {
        Unit__Base_Stat TargetStat = other.GetComponent<UnitBaseEngine>()._unit_Stat;

        yield return new WaitForSeconds(RemainingTime);

        TargetStat.__PUB_Move_Speed /= MovementSpeedModifier;
        TargetStat.__PUB_Rotation_Speed /= RotationSpeedModifier;
    }

    /** This timer is activated as soon as ultimate skill is spawned */
    private IEnumerator Timer()
    {
        // wait for 'SkillDuration'
        Collider.enabled = true;
        yield return new WaitForSeconds(SkillDuration);

        // disable skill effect & apply remaining skill effect on objects in collision
        Collider.enabled = false;
        for (int i = 0; i < ObjectsInCollision.Count; i++)
            StartCoroutine(RemainedSkillEffect(ObjectsInCollision[i]));
        yield return new WaitForSeconds(RemainingTime);

        // wait for objects having remaining skill effect & destroy ultimate skill object
        Destroy(gameObject);
    }
}
