// using UnityEngine;
// using UnityEditor;

// [CustomEditor(typeof(PortalManager))]
// public class PortalManagerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI(); // Draws the default inspector

//         PortalManager script = (PortalManager)target;

//         if (GUILayout.Button("Create Random Portal"))
//         {
//             Vector3 randomPosition = new Vector3(
//                 Random.Range(-1, 1), // X position
//                 Random.Range(-0.32f, 0.32f), // Y position
//                 Random.Range(-1, 1)  // Z position
//             );

//             Quaternion randomRotation = Quaternion.Euler(
//                 Random.Range(-45, 45),  // X rotation
//                 Random.Range(0, 360), // Y rotation
//                 Random.Range(0, 360)  // Z rotation
//             );

//             script.CreatePortal(randomPosition, randomRotation);
//         }
//     }
// }
