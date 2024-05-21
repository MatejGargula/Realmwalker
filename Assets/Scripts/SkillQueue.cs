using System.Collections;
using Realmwalker.Combat;
using UnityEngine;

internal enum QueueType
{
    TAIL, // last queue - the slowest skill are place here
    MIDDLE, // middle of the queue - has previous and next queue
    HEAD // first/main queue - skills that will be played the following turn 
}

public class SkillQueue : MonoBehaviour
{
    [Header("Config")] [SerializeField] private QueueType type;

    [SerializeField] public int skillsInQueue;
    [SerializeField] private int maxSkillsInQueue = 4;
    [SerializeField] private float skillDelay = 0.5f;
    [SerializeField] public bool empty;
    [SerializeField] public bool full;
    [SerializeField] private bool debug;
    [SerializeField] private SkillQueue nextQueue;
    [SerializeField] private SkillQueue previousQueue;

    [Header("Support Objects & prefabs")] [SerializeField]
    private GameObject queuedSkillPrefab;

    [SerializeField] private OldCombatManager cm;

    private void Awake()
    {
        skillsInQueue = 0;
        empty = true;
        full = false;
        cm = FindObjectOfType<OldCombatManager>();
    }

    /// <summary>
    ///     Adds a skill in the skill queue and instantiates a new gameobject QueuedSkill
    /// </summary>
    /// <param name="skill">SKill to be enqueued.</param>
    public void EnqueueSkill(QueuedSkill skill)
    {
        if (!full)
        {
            //GameObject queuedSkillObject = Instantiate(queuedSkillPrefab, gameObject.transform, false);
            //queuedSkillObject.AddComponent<QueuedSkill>();
            //QueuedSkill skillInQueue = queuedSkillObject.GetComponent<QueuedSkill>();
            //skillInQueue.CopyQueuedSkill(skill);
            //skillInQueue.Init();
            //skillsInQueue++;
            //if(skillsInQueue == maxSkillsInQueue)
            //{
            //    full = true;
            //}
        }
        else
        {
            if (debug) Debug.Log("Cannot enqueue skill: Queue is full");
        }

        if (empty) empty = false;
    }

    /// <summary>
    ///     Removes the first skill from the queue and destroys the QueuedSKill gameobject.
    /// </summary>
    /// <returns>Returns skill from the queue to be played</returns>
    public QueuedSkill DequeueSkill()
    {
        Debug.Log("Dequeuing");
        if (!empty)
        {
            var child = gameObject.transform.GetChild(0);
            var skill = child.gameObject.GetComponent<QueuedSkill>();
            Destroy(gameObject.transform.GetChild(0).gameObject);
            full = false;
            skillsInQueue--;
            if (skillsInQueue == 0) empty = true;
            return skill;
        }

        return null;
    }

    public void PlayAllSKillsInQueueu()
    {
        StartCoroutine(PlaySkills());
    }

    private IEnumerator PlaySkills()
    {
        var sum = skillsInQueue;
        for (var i = 0; i < sum; i++)
        {
            var child = gameObject.transform.GetChild(0);
            var skill = child.gameObject.GetComponent<QueuedSkill>();
            //skill.PlaySkill();
            Destroy(child.gameObject);
            yield return new WaitForSeconds(skillDelay);
        }

        skillsInQueue = 0;
        StartCoroutine(QueueShift());
    }

    /// <summary>
    ///     Shifts all skills in this queue into the next one.
    ///     TODO: Implement skill checks for stunned character.(queued skills have a pointer to the origin character)
    /// </summary>
    public void SkillShift()
    {
        if (nextQueue != null)
        {
            var skillCount = transform.childCount;
            //GameObject[] skillsCurrent = new GameObject[skillCount];
            for (var i = 0; i < skillCount; i++)
            {
                var child = transform.GetChild(transform.childCount - 1);
                child.SetParent(nextQueue.gameObject.transform);
            }

            nextQueue.skillsInQueue = nextQueue.transform.childCount;
            if (nextQueue.skillsInQueue == nextQueue.maxSkillsInQueue)
                nextQueue.full = full;
            else if (nextQueue.skillsInQueue == 0) nextQueue.empty = empty;
        }
    }

    /// <summary>
    ///     This function should be only called on a queue with type HEAD. It will shift all the queues
    /// </summary>
    public IEnumerator QueueShift()
    {
        if (type != QueueType.HEAD)
        {
            var skillCount = transform.childCount;
            //GameObject[] skillsCurrent = new GameObject[skillCount];
            for (var i = 0; i < skillCount; i++)
            {
                var child = transform.GetChild(0);
                child.SetParent(nextQueue.gameObject.transform);
                yield return new WaitForSeconds(skillDelay);
            }
        }

        if (type != QueueType.TAIL)
        {
            skillsInQueue = previousQueue.transform.childCount;
            if (skillsInQueue == maxSkillsInQueue)
            {
                full = true;
            }
            else
            {
                full = false;
                if (skillsInQueue == 0)
                    empty = true;
                else
                    empty = false;
            }

            if (previousQueue != null) StartCoroutine(previousQueue.QueueShift());
        }
        else
        {
            skillsInQueue = transform.childCount;
            empty = true;
            full = false;
            cm.wait = false;
        }
    }
}