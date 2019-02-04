using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NY_LiftSelectorButton : MonoBehaviour
{
    public int floorNumber;
    public bool up=false;
    public bool down=false;
    [Space(10.0f)]
    [SerializeField]
    private bool isGroundFloor = false;
    [SerializeField]
    private bool isTopFloor = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void UpButtonPressedAt()
    {
        if (isTopFloor)
        {
            return;
        }
        if(NY_Lift_Manager.Instance != null)
        {
            NY_Lift_Manager.Instance.UpGoingLiftSelectAtFloor(floorNumber);
        }
       
    }
    public void DownButtonPressedAt()
    {

        if (isGroundFloor)
        {
            return;
        }
        if (NY_Lift_Manager.Instance != null)
        {
            NY_Lift_Manager.Instance.DownGoingLiftSelectAtFloor(floorNumber);
        }
       
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (up)
        {
            up = false;
            UpButtonPressedAt();
        }
        if (down)
        {
            DownButtonPressedAt();
            down = false;
        }

    }
#endif
}
