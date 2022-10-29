using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator
{   
    public class ClassService<TMono> where TMono : MonoBehaviour
    {
        private static Dictionary<int, object> Services = new Dictionary<int, object>();                

        public static void  Register(int ID = -1, TMono obj = null)
        {
            if (ID == -1) ID = Random.Range(0, 999999);

            if (!Services.ContainsKey(ID))
            {
                if (obj == null)
                {
                    TMono mono = (TMono)Object.FindObjectOfType(typeof(TMono));
                    Services.Add(ID, mono);
                }
                else
                {
                    if (obj.GetType() == typeof(TMono))
                        Services.Add(ID, obj);                    
                }       
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("Service with ID "+ID+" already exists!");
#endif
            }
        }
        public static TMono Resolve(int TargetID = -1)
        {
            TMono rs = default;
            if (TargetID != -1)
            {
                if (Services.ContainsKey(TargetID))
                    rs = (TMono)Services[TargetID];
            }
            else
            {
                foreach (var item in Services)
                {
                    if (item.Value.GetType() == (typeof(TMono)))
                    {
                        rs = (TMono)item.Value;
                        break;
                    }
                }
            }
            if (rs == default)
                return null;
            else
                return rs;
        }
        public static void  Dispose(int ID = -1)
        {
            int removeID = ID;
            if (removeID == -1)
            {                
                foreach (var item in Services)
                {
                    if (item.Value.GetType() == (typeof(TMono)))
                    {
                        removeID = item.Key;
                        break;
                    }
                }
            }            
            bool isRemove = Services.Remove(removeID);
            if (!isRemove)
            {
#if UNITY_EDITOR
                Debug.LogWarning("ID: " + ID + " Not found!");
#endif
            }
        }
    }    
}

