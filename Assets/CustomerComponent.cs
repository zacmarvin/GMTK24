using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerComponent : MonoBehaviour
{
    public enum CustomerState
    {
        WalkingUp,
        WaitingInLine,
        WaitingOnFood
    }

    private Vector3 cornerPosition = new Vector3(1, 1, 5.5f);
    
    private Vector3 linePosition1 = new Vector3(1, 1, 2f);
    
    private Vector3 linePosition2 = new Vector3(1, 1, 3f);
    
    private Vector3 linePosition3 = new Vector3(1, 1, 4f);
    
    private Vector3 waitPosition1 = new Vector3(-2.5f, 1, 2.5f);
    
    private Vector3 waitPosition2 = new Vector3(-2.5f, 1, 4f);
    
    private Vector3 waitPosition3 = new Vector3(-1.25f, 1, 4f);
    
    public CustomerState CurrentState = CustomerState.WalkingUp;
    
    public float moveSpeed = 1.5f;
    
    public int linePosition = 3;

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case CustomerState.WalkingUp:
                transform.position = Vector3.MoveTowards(transform.position, cornerPosition, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, cornerPosition) < 0.1f)
                {
                    CurrentState = CustomerState.WaitingInLine;
                }
                break;
            case CustomerState.WaitingInLine:
                if (linePosition == 3)
                {
                    transform.position = Vector3.MoveTowards(transform.position, linePosition3, moveSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, linePosition3) < 0.1f)
                    {
                        linePosition = 2;
                    }
                }
                else if (linePosition == 2)
                {
                    transform.position = Vector3.MoveTowards(transform.position, linePosition2, moveSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, linePosition2) < 0.1f)
                    {
                        linePosition = 1;
                    }
                }
                else if (linePosition == 1)
                {
                    transform.position = Vector3.MoveTowards(transform.position, linePosition1, moveSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, linePosition1) < 0.1f)
                    {
                        CurrentState = CustomerState.WaitingOnFood;
                    }
                }
                break;
            case CustomerState.WaitingOnFood:
                break;
        }
    }
}
