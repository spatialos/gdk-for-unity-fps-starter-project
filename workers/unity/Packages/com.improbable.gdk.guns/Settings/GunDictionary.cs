using UnityEngine;

namespace Improbable.Gdk.Guns
{
    [CreateAssetMenu(menuName = "Improbable/Gun Config/Gun Dictionary", order = 0)]
    public class GunDictionary : ScriptableObject
    {
        public static GunDictionary Instance { private get; set; }

        [SerializeField] private GunSettings[] gunsList;

        public static GunSettings Get(int index)
        {
            if (Instance == null)
            {
                Debug.LogError("The Gun Dictionary has not been set.");
                return null;
            }

            if (index < 0 || index >= Count)
            {
                Debug.LogErrorFormat("The index {0} is outside of the dictionary's range (size {1}).", index, Count);
                return null;
            }

            return Instance.gunsList[index];
        }

        public static int Count => Instance.gunsList.Length;
    }
}
