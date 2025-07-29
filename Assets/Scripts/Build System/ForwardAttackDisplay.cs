using UnityEngine;

public class ForwardAttackDisplay : MonoBehaviour
{
    [SerializeField] private LineRenderer leftLine;
    [SerializeField] private LineRenderer rightLine;
    [SerializeField] private float attackRange;

    public void CreateLine(bool showLine, float newRange)
    {
        leftLine.enabled = showLine;
        rightLine.enabled = showLine;

        if (showLine == false)
            return;
            
        attackRange = newRange;
        UpdateLine();
    }

    public void UpdateLine()
    {
        DrawLine(leftLine);
        DrawLine(rightLine);
    }

    private void DrawLine(LineRenderer line)
    {
        Vector3 start = line.transform.position;
        Vector3 end = start + (transform.forward * attackRange);

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
