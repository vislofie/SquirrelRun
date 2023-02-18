using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WolfSenses))]
public class EntitySensesVisualization : Editor
{
    private void OnSceneGUI()
    {
        WolfSenses senses = (WolfSenses)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(senses.transform.position, Vector3.up, Vector3.forward, 360, senses.ViewRadius);

        Vector3 viewAngleDirA = senses.DirFromAngle(senses.ViewAngle / 2, false);
        Vector3 viewAngleDirB = senses.DirFromAngle(-senses.ViewAngle / 2, false);

        Handles.DrawLine(senses.transform.position, senses.transform.position + viewAngleDirA * senses.ViewRadius);
        Handles.DrawLine(senses.transform.position, senses.transform.position + viewAngleDirB * senses.ViewRadius);

        if (senses.PrioritizedVisibleTarget != null)
            Handles.DrawLine(senses.transform.position, senses.PrioritizedVisibleTarget.position);
    }
}
