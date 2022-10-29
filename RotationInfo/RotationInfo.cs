using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationInfo : MonoBehaviour
{
    public bool debug;
    public bool workItself;
    public bool checkOtherTransform;

    public Transform otherTransform;

    public DebugType debugType;

    private int x;
    private int y;

    private float pX = 0;
    private float pY = 0;

    [Tooltip("Represents which direction the object is rotating.")]
    private TurnDirection turnDirection = TurnDirection.ToBack;
    [Tooltip("Represents whether the object rotates clockwise or counterclockwise.")]
    private TurnType turnType = TurnType.Clockwise;
    [Tooltip("Represents absolute direction the object.")]
    private Direction direction = Direction.Back;
    [Tooltip("Shows which region the object is in the vector plane.")]
    private Area area = Area.Area1;

    private void OnValidate()
    {
        if (!checkOtherTransform)
            otherTransform = null;
    }

    private void Update()
    {
        if (debug)
        {
            Debug();
        }
        if (workItself && !checkOtherTransform)
        {
            CheckRotation(transform);
        }
        else if (checkOtherTransform && workItself)
        {
            CheckRotation(otherTransform);
        }
    }

    public TurnDirection GetTurnDirection() => turnDirection;

    public Area GetArea() => area;
    public int GetAreaAsNumber() => (int)area;

    public TurnType GetTurnType() => turnType;
    public int GetTurnTypeAsBit() => (int)turnType;

    public Direction GetDirection() => direction;
    public Vector3 GetDirectionAsVector3()
    {
        switch (direction)
        {
            case Direction.Forward:
                return Vector3.forward;                
            case Direction.Up:
                return Vector3.up;
            case Direction.Back:
                return Vector3.back;
            case Direction.Down:
                return Vector3.down;
            default:
                return Vector3.zero;
        }
    }

    private void CheckRotation(Transform tr)
    {        
        x = Mathf.FloorToInt(Vector3.Angle(tr.forward, Vector3.forward));
        y = Mathf.FloorToInt(Vector3.Angle(tr.forward, Vector3.up));        

        if (x == 0 && y == 90)
            direction = Direction.Forward;        
        else if (x == 90 && y == 0)
            direction = Direction.Up;        
        else if (x == 180 && y == 90)
            direction = Direction.Back;        
        else if (x == 90 && y == 180)
            direction = Direction.Down;        

        if ((x > 0 && x < 90) && (y > 0 && y < 90))
        {
            area = Area.Area1;
           
            if (x > pX && y < pY)
            {
                turnDirection = TurnDirection.ToUp;
                turnType = TurnType.CounterClockwise;                               
            }
            else if (x < pX && y > pY)
            {
                turnDirection = TurnDirection.ToForward;
                turnType = TurnType.Clockwise;
            }
            pX = x;
            pY = y;
        }
        else if ((x > 90 && x < 180) && (y > 0 && y < 90))
        {
            area = Area.Area2;
            
            if (x > pX && y > pY)
            {
                turnDirection = TurnDirection.ToBack;
                turnType = TurnType.CounterClockwise;
            }
            else if (x < pX && y < pY)
            {
                turnDirection = TurnDirection.ToUp;
                turnType = TurnType.Clockwise;
            }
            pX = x;
            pY = y;
        }
        else if ((x > 90 && x < 180) && (y > 90 && y < 180))
        {
            area = Area.Area3;
            
            if (x < pX && y > pY)
            {
                turnDirection = TurnDirection.ToDown;
                turnType = TurnType.CounterClockwise;
            }
            else if (x > pX && y < pY)
            {
                turnDirection = TurnDirection.ToBack;
                turnType = TurnType.Clockwise;
            }
            pX = x;
            pY = y;
        }
        else if ((x > 0 && x < 90) && (y > 90 && y < 180))
        {
            area = Area.Area4;
            
            if (x < pX && y < pY)
            {
                turnDirection = TurnDirection.ToForward;
                turnType = TurnType.CounterClockwise;
            }
            else if (x > pX && y > pY)
            {
                turnDirection = TurnDirection.ToDown;
                turnType = TurnType.Clockwise;
            }
            pX = x;
            pY = y;
        }        
    }

    private void Debug()
    {
#if UNITY_EDITOR
        if (debugType == DebugType.Lebug)
        {
            //DIRECTION
            Lebug.Log("Direction", direction);

            //AREA
            Lebug.Log("Area", area);

            //TURN DIRECTION
            Lebug.Log("Move Direction", turnDirection);

            //TURN TYPE
            Lebug.Log("Turn Type", turnType);

            //ANGLES
            Lebug.Log("Forward to forward angle: ", x);
            Lebug.Log("Forward to up angle: ", y);
        }
        else if (debugType == DebugType.Debug)
        {
            //DIRECTION
            UnityEngine.Debug.Log("Direction" + direction);

            //AREA
            UnityEngine.Debug.Log("Area" + area);

            //TURN DIRECTION
            UnityEngine.Debug.Log("Move Direction" + turnDirection);

            //TURN TYPE
            UnityEngine.Debug.Log("Turn Type" + turnType);

            //ANGLES
            UnityEngine.Debug.Log("Forward to forward angle: " + x);
            UnityEngine.Debug.Log("Forward to up angle: " + y);
        }
#endif
    }

    public enum TurnDirection { ToForward, ToUp, ToBack, ToDown }
    public enum TurnType { Clockwise = 1, CounterClockwise = -1 }
    public enum Direction { Forward, Up, Back, Down }
    public enum Area { Area1 = 1, Area2 = 2, Area3 = 3, Area4 = 4 }

    public enum DebugType { Debug, Lebug }    
}
