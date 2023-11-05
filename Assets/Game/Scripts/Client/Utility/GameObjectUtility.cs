using UnityEngine;

namespace TWT.Utility
{
    public static class GameObjectUtility
    {
        public static Bounds CalculateLocalBounds(GameObject gameObj)
        {
            Quaternion currentRotation = gameObj.transform.rotation;
            gameObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            Bounds bounds = new Bounds(gameObj.transform.position, Vector3.zero);

            foreach (Renderer renderer in gameObj.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            Vector3 localCenter = bounds.center - gameObj.transform.position;
            bounds.center = localCenter;
            //DebugExtension.Log("The local bounds of this model is " + bounds);

            gameObj.transform.rotation = currentRotation;

            return bounds;
        }

        public static Vector3 StringToVector3(string str)
        {
            string[] split = str.Split(',');
            Vector3 result = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            return result;
        }

        public static string Vector3ToString(Vector3 vector)
        {
            string result = vector.x + ", " + vector.y + ", " + vector.z;
            return result;
        }
    }
}