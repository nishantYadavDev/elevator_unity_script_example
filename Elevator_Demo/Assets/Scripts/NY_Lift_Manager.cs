using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NY_Lift_Manager : MonoBehaviour
{
    public static NY_Lift_Manager Instance;
    public NY_Elevator elevator_1, elevator_2;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpGoingLiftSelectAtFloor(int floorNumber)
    {
        Debug.Log("Up going lift selected at floorNumber " + floorNumber);
        FindWhichElevatorIsClosestTo(floorNumber, true);
    }

    public void DownGoingLiftSelectAtFloor(int floorNumber)
    {
        Debug.Log("Down going lift selected at floorNumber " + floorNumber);
        FindWhichElevatorIsClosestTo(floorNumber, false);
    }

    private void FindWhichElevatorIsClosestTo(int floorNumber,bool dirUP)
    {
        //compare the floor distance

        int e1 = Mathf.Abs(elevator_1.CurrentFloorAt - floorNumber);
        int e2 = Mathf.Abs(elevator_2.CurrentFloorAt - floorNumber);
        if (e1 > e2)
        {
            //means elevator1 is far in floor numbering
        }
        else if (e2 > e1)
        {
            //means elevator2 is far in floor numbering
        }
        else
        {
            //both are at equal distance from the user
        }
        //compare the time distance


        float e1_time = elevator_1.CalculateTimeToReachFloor(floorNumber);
        float e2_time = elevator_2.CalculateTimeToReachFloor(floorNumber);

        if (e1_time > e2_time)
        {
            //means elevator1 is far in floor numbering
            Debug.Log("Move Lift 2 towards user with time 1 and time 2  : "+e1_time +"   "+e2_time);
            elevator_2.StartElevator(floorNumber, dirUP);
        }
        else if (e2_time > e1_time)
        {
            //means elevator2 is far in floor numbering
            Debug.Log("Move Lift 1 towards user with time 1 and time 2  : " + e1_time + "   " + e2_time);
            elevator_1.StartElevator(floorNumber, dirUP);
        }
        else
        {
            //both are at equal distance from the user
            Debug.Log("Move Lift 1 0r Lift2 towards user");
            elevator_1.StartElevator(floorNumber, dirUP);
        }
        if (dirUP)
        {
            //means user wants to go to  upper floors
            
        }
        else
        {
            //means user wants to go to  lower floors

            //compare the floor distance


            //compare the time distance
        }
    }
}
