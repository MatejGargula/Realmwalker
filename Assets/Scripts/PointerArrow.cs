using UnityEngine;

public class PointerArrow : MonoBehaviour
{
    [SerializeField] private Transform[] positions;

    public void SetNewPosition(int number)
    {
        transform.position = positions[number].position;
        transform.SetParent(positions[number].transform);
    }
}