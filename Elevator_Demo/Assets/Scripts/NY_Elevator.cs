using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NY_Elevator : MonoBehaviour
{
    public bool isWorking = false;
    [Space]
    public bool startElevator = false;
    public ElevatorStatus currently;
    bool movingUp = false;
    public float openWaitTime = 5.0f;
    public float closedWaitTime = 2.0f;

    public float speedMovement = 3.1f;
    public NY_ControlPanel elevControlPanel;
    public int CurrentFloorAt { get { return elevControlPanel.currentFloor; } }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartElevator()
    {
        isWorking = true;
        // currently = ElevatorStatus.moving_up;
        StartCoroutine(MoveLift());
    }

    IEnumerator MoveLift()
    {
        while (isWorking)
        {
            yield return new WaitForSeconds(speedMovement);

            if (currently == ElevatorStatus.moving_up)
            {
                if (elevControlPanel.CheckWhetherToStopOrNotAt(++elevControlPanel.currentFloor))
                {
                    currently = ElevatorStatus.open;
                    yield return new WaitForSeconds(openWaitTime);
                    currently = ElevatorStatus.closed;
                    yield return new WaitForSeconds(closedWaitTime);
                    currently = ElevatorStatus.moving_up;
                    elevControlPanel.RemoveDestin(elevControlPanel.currentFloor);
                }
            }
            if (currently == ElevatorStatus.moving_down)
            {
                if (elevControlPanel.CheckWhetherToStopOrNotAt(--elevControlPanel.currentFloor))
                {
                    currently = ElevatorStatus.open;
                    yield return new WaitForSeconds(openWaitTime);
                    currently = ElevatorStatus.closed;
                    yield return new WaitForSeconds(closedWaitTime);
                    currently = ElevatorStatus.moving_down;
                    elevControlPanel.RemoveDestin(elevControlPanel.currentFloor);
                }
            }

            if (elevControlPanel.ReachedTopFloor())
            {
                currently = ElevatorStatus.moving_down;
            }
            if (elevControlPanel.ReachedGroundFloor())
            {
                currently = ElevatorStatus.moving_up;
            }
        }

    }

    public void StartElevator(int dest,bool dirUP)
    {
       
        // currently = ElevatorStatus.moving_up;
        if (!isWorking)
        {
            isWorking = true;
            StartCoroutine(MoveLift(dest, dirUP));
        }
        else
        {
            elevControlPanel.SelectFloor(dest);
            
        }
    }
   

    IEnumerator MoveLift(int dest,bool dirUP)
    {
        elevControlPanel.SelectFloor(dest);
        if (currently == ElevatorStatus.stopped)
        {
            if (dirUP)
            {
                currently = ElevatorStatus.moving_up;
                if (elevControlPanel.ReachedTopFloor())
                {
                    currently = ElevatorStatus.moving_down;
                }
            }
            else
            {
                currently = ElevatorStatus.moving_down;
                if (elevControlPanel.ReachedGroundFloor())
                {
                    currently = ElevatorStatus.moving_up;
                }
            }
        }
        while (isWorking)
        {
            yield return new WaitForSeconds(speedMovement);

            if (currently == ElevatorStatus.moving_up)
            {
                if (elevControlPanel.CheckWhetherToStopOrNotAt(elevControlPanel.currentFloor))
                {
                    currently = ElevatorStatus.open;
                    yield return new WaitForSeconds(openWaitTime);
                    currently = ElevatorStatus.closed;
                    yield return new WaitForSeconds(closedWaitTime);
                    currently = ElevatorStatus.moving_up;
                    elevControlPanel.RemoveDestin(elevControlPanel.currentFloor);
                    if (elevControlPanel.destinFloors.Count == 0)
                    {
                        isWorking = false;
                        currently = ElevatorStatus.stopped;
                        continue;
                    }
                }
                
                elevControlPanel.currentFloor = elevControlPanel.currentFloor + 1;
            }
            if (currently == ElevatorStatus.moving_down)
            {
                if (elevControlPanel.CheckWhetherToStopOrNotAt(elevControlPanel.currentFloor))
                {
                    currently = ElevatorStatus.open;
                    yield return new WaitForSeconds(openWaitTime);
                    currently = ElevatorStatus.closed;
                    yield return new WaitForSeconds(closedWaitTime);
                    currently = ElevatorStatus.moving_down;
                    elevControlPanel.RemoveDestin(elevControlPanel.currentFloor);
                    if (elevControlPanel.destinFloors.Count == 0)
                    {
                        isWorking = false;
                        currently = ElevatorStatus.stopped;
                        continue;
                    }

                }
                elevControlPanel.currentFloor = elevControlPanel.currentFloor - 1;
            }

            if (elevControlPanel.ReachedTopFloor())
            {
                currently = ElevatorStatus.moving_down;
            }
            if (elevControlPanel.ReachedGroundFloor())
            {
                currently = ElevatorStatus.moving_up;
            }
            if (elevControlPanel.destinFloors.Count == 0)
            {
                isWorking = false;
                currently = ElevatorStatus.stopped;
            }
        }

    }

    public float CalculateTimeToReachFloor(int destinfloorNumber)
    {
        int floorsBetween = CurrentFloorAt - destinfloorNumber;
        float timeTaken = 0.0f;
        if (floorsBetween < 0)
        {
            //means it is past us check for moving upwards
            if (elevControlPanel.destinFloors.Count == 0)
            {
                //means we have served everyone and at static
                timeTaken = Mathf.Abs(floorsBetween) * speedMovement + openWaitTime;
            }
            else if (!movingUp)
            {
                //means going in opposite direction from us
                int[] floors = elevControlPanel.destinFloors.ToArray();
                Array.Sort(floors);
                int maxFloorServing = floors[(floors.Length - 1)];
                List<int> floorsGreaterCurnt = new List<int>();
                List<int> floorsLowerCurnt = new List<int>();
                int j = 0;
                int k = 0;
                for (int i = 0; i < floors.Length; i++)
                {
                    if (floors[i] > CurrentFloorAt && floors[i] <= destinfloorNumber)
                    {
                        floorsGreaterCurnt.Add( floors[i]);
                    }
                    else if (floors[i] < CurrentFloorAt )
                    {
                        floorsLowerCurnt.Add(floors[i]);
                    }
                }
                if (floorsLowerCurnt.Count > 0)
                {
                    timeTaken += (Mathf.Abs(floorsLowerCurnt[floorsLowerCurnt.Count - 1] - CurrentFloorAt)) * speedMovement + openWaitTime + closedWaitTime;
                    for (int i = floorsLowerCurnt.Count - 2; i >= 0; i--)
                    {
                        int distnk = floorsLowerCurnt[i] - floorsLowerCurnt[i + 1];
                        timeTaken += (Mathf.Abs(distnk) * speedMovement) + openWaitTime + closedWaitTime;
                    }
                    timeTaken += (Mathf.Abs(floorsLowerCurnt[0] - CurrentFloorAt) * speedMovement) + openWaitTime;
                }
                if (floorsGreaterCurnt.Count > 0)
                {
                    timeTaken += (Mathf.Abs(floorsGreaterCurnt[0] - CurrentFloorAt)) * speedMovement + openWaitTime + closedWaitTime;
                    for (int i = 1; i < floorsGreaterCurnt.Count; i++)
                    {
                        int distnk = floorsGreaterCurnt[i] - floorsGreaterCurnt[i - 1];
                        timeTaken += (Mathf.Abs(distnk) * speedMovement) + openWaitTime + closedWaitTime;
                    }
                    timeTaken += (Mathf.Abs(floorsGreaterCurnt[floorsGreaterCurnt.Count-1] - destinfloorNumber) * speedMovement) + openWaitTime;

                }


            }
            else
            {
                //means coming towards us in upwward direction
                int[] floors = elevControlPanel.destinFloors.ToArray();
                Array.Sort(floors);
                List<int> floorsGreaterCurnt = new List<int>();

                int j = 0;
                for (int i = 0; i < floors.Length; i++)
                {
                    if (floors[i] > CurrentFloorAt && floors[i] <= destinfloorNumber)
                    {
                        floorsGreaterCurnt.Add(floors[i]);
                    }
                    
                }
                if (floorsGreaterCurnt.Count > 0)
                {
                    timeTaken += (Mathf.Abs(floorsGreaterCurnt[0] - CurrentFloorAt)) * speedMovement + openWaitTime + closedWaitTime;
                    for (int i = 1; i < floorsGreaterCurnt.Count; i++)
                    {
                        int distnk = floorsGreaterCurnt[i] - floorsGreaterCurnt[i - 1];
                        timeTaken += (Mathf.Abs(distnk) * speedMovement) + openWaitTime + closedWaitTime;
                    }
                    timeTaken += (Mathf.Abs(floorsGreaterCurnt[floorsGreaterCurnt.Count-1] - destinfloorNumber) * speedMovement) + openWaitTime;
                }
            }
        }
        else if (floorsBetween > 0)
        {
            //means it is below us check for moving downward
            if (elevControlPanel.destinFloors.Count == 0)
            {
                //means we have served everyone and at static
                timeTaken = floorsBetween * speedMovement + openWaitTime;
            }
            else if (movingUp)
            {
                //means going in opposite direction from us
                int[] floors = elevControlPanel.destinFloors.ToArray();
                Array.Sort(floors);
                int maxFloorServing = floors[(floors.Length - 1)];
                List<int> floorsGreaterCurnt = new List<int>();
                List<int> floorsLowerCurnt = new List<int>();
                int j = 0;
                int k = 0;
                for (int i = 0; i < floors.Length; i++)
                {
                    if (floors[i] > CurrentFloorAt)
                    {
                        floorsGreaterCurnt.Add( floors[i]);
                    }
                    else if (floors[i] < CurrentFloorAt && floors[i] >= destinfloorNumber)
                    {
                        floorsLowerCurnt.Add(floors[i]);
                    }
                }
                if (floorsGreaterCurnt.Count > 0)
                {
                    timeTaken = (Mathf.Abs(floorsGreaterCurnt[0] - CurrentFloorAt)) * speedMovement + openWaitTime + closedWaitTime;
                    for (int i = 1; i < floorsGreaterCurnt.Count; i++)
                    {
                        int distnk = floorsGreaterCurnt[i] - floorsGreaterCurnt[i - 1];
                        timeTaken += (Mathf.Abs(distnk) * speedMovement) + openWaitTime + closedWaitTime;
                    }
                }
             
                if (floorsLowerCurnt.Count > 0)
                {
                    timeTaken += (Mathf.Abs(floorsLowerCurnt[floorsLowerCurnt.Count - 1] - maxFloorServing)) * speedMovement + openWaitTime + closedWaitTime;
                    for (int i = floorsLowerCurnt.Count - 2; i >= 0; i--)
                    {
                        int distnk = floorsLowerCurnt[i] - floorsLowerCurnt[i + 1];
                        timeTaken += (Mathf.Abs(distnk) * speedMovement) + openWaitTime + closedWaitTime;
                    }
                    timeTaken += (Mathf.Abs(floorsLowerCurnt[0] - destinfloorNumber) * speedMovement) + openWaitTime;
                }
            }
            else
            {
                //means coming towards us
                int[] floors = elevControlPanel.destinFloors.ToArray();
                Array.Sort(floors);  
                List<int> floorsLowerCurnt = new List<int>();
                
                int k = 0;
                for (int i = 0; i < floors.Length; i++)
                {                   
                     if (floors[i] < CurrentFloorAt && floors[i] >= destinfloorNumber)
                    {
                        floorsLowerCurnt.Add(floors[i]);
                    }
                }      
                if (floorsLowerCurnt.Count > 0)
                {
                    timeTaken += (Mathf.Abs(floorsLowerCurnt[floorsLowerCurnt.Count - 1] - CurrentFloorAt)) * speedMovement + openWaitTime + closedWaitTime;
                    for (int i = floorsLowerCurnt.Count - 2; i >= 0; i--)
                    {
                        int distnk = floorsLowerCurnt[i] - floorsLowerCurnt[i + 1];
                        timeTaken += (Mathf.Abs(distnk) * speedMovement) + openWaitTime + closedWaitTime;
                    }
                    timeTaken += (Mathf.Abs(floorsLowerCurnt[0] - destinfloorNumber) * speedMovement) + openWaitTime;
                }
            }
        }
        else
        {
            //means on the same floor

        }
        return timeTaken;
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (startElevator)
        {

            StartElevator();
            startElevator = false;
        }
        if (elevControlPanel.initialzeFloors)
        {
            elevControlPanel.InitializeFloors();

            elevControlPanel.initialzeFloors = false;
        }

        elevControlPanel.ButtonStatus();
    }

#endif
}
[System.Serializable]
public class NY_ControlPanel
{
    public int currentFloor = -1;
    public int totalFloors = 0;
#if UNITY_EDITOR

    public bool initialzeFloors;
    [Space]
    public bool[] buttons;
#endif
    public List<int> destinFloors;
    public int[] floors;
    public bool CheckWhetherToStopOrNotAt(int floorNumber)
    {
        if (destinFloors.IndexOf(floorNumber) >= 0)
        {
            return true;
        }
        return false;
    }
    public bool RemoveDestin(int num)
    {
        return destinFloors.Remove(num);
    }
    public bool ReachedTopFloor()
    {
        if (currentFloor >= totalFloors - 1)
        {
            return true;
        }
        return false;
    }
    public bool ReachedGroundFloor()
    {
        if (currentFloor == 0)
        {
            return true;
        }
        return false;
    }
    public void InitializeFloors()
    {
        floors = new int[totalFloors];
        for (int i = 0; i < floors.Length; i++)
        {
            floors[i] = i;

        }
        buttons = new bool[floors.Length];
    }
    public void SelectFloor(int num)
    {
        if (num >= 0 && num < floors.Length)
        {
            if (destinFloors.IndexOf(num) < 0)
            {
                destinFloors.Add(num);
            }
        }
        else
        {
            Debug.LogError("Could Not Select this number floor");
        }
    }
    public void ButtonStatus()
    {
        int i = CheckWhichButtonPressed();

        if (i >= 0)
        {
            SelectFloor(i);
        }

    }
    private int CheckWhichButtonPressed()
    {
        int k = -1;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i])
            {
                k = i;
                buttons[i] = false;
                break;
            }

        }
        return k;
    }
}
public enum ElevatorStatus
{
    stopped,
    closed,
    open,
    moving_up,
    moving_down,
   
}
