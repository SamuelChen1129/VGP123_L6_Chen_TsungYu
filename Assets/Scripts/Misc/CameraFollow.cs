using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float minXClamp = -0.02f;
    public float maxXClamp = 237.81f;

    public float minYClamp = 0.04f;
    public float maxYClamp = 11.55f;

    //this fx always runs after fixed update - unity specifies this si where camera movement should happen
    private void LateUpdate()
    {
        Vector3 cameraPos = transform.position;

        cameraPos.x = Mathf.Clamp(player.transform.position.x, minXClamp, maxXClamp);
        cameraPos.y = Mathf.Clamp(player.transform.position.y, minYClamp, maxYClamp);

        transform.position = cameraPos;
    }
}