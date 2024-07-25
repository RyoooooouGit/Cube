using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Transform playerTransform;
    private Rigidbody playerRigidbody;
    private RaycastHit hitBelowInfo, hitSideInfo, hitUpSideInfo, hitUpUpSideInfo;
    private bool ifMoving = false;
    public bool ifTeleporting = false;
    [Range(0.5f, 5)]
    public float rotateSpeed = 2;
    public Vector3 gravityDirection = new Vector3(0, -1, 0);
    private Transform startTransform;
    public float gravityStrength = 9.8f;
    private Vector3 gravityForce;
    private Transform levelTransform, levelNowTransform, goalTransform;
    private int levelIndex = 1;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        playerTransform = GetComponent<Transform>();
        playerRigidbody = GetComponent<Rigidbody>();
        levelTransform = GameObject.Find("Levels").GetComponent<Transform>();
    }
    private void Start()
    {
        KeyManager.instance.move += move;
        KeyManager.instance.restart += restart;
        gravityForce = gravityDirection.normalized * gravityStrength;
        levelNowTransform = levelTransform.GetChild(levelIndex - 1);
        startTransform = levelNowTransform.Find("Start");
        goalTransform = levelNowTransform.Find("Goal");
        goalTransform.gameObject.SetActive(true);
    }

    #region character animation
    // 使人物的坐标与旋转摆正
    private void straighten()
    {
        Vector3 finalAngle = playerTransform.localEulerAngles;
        Vector3 finalPosition = playerTransform.position;
        playerTransform.rotation = Quaternion.Euler(90 * Mathf.Ceil((finalAngle.x - 45) / 90), 90 * Mathf.Ceil((finalAngle.y - 45) / 90), 90 * Mathf.Ceil((finalAngle.z - 45) / 90));
        playerTransform.position = new Vector3(Mathf.Ceil((finalPosition.x - 0.2f) / 0.4f) * 0.4f, Mathf.Ceil((finalPosition.y - 0.4f) / 0.4f) * 0.4f + 0.2f, Mathf.Ceil((finalPosition.z - 0.2f) / 0.4f) * 0.4f);
    }
    // 当按下键盘wasd后调用下面两个函数/协程进行旋转
    public void move(Vector3 way)
    {
        if (!ifMoving && !ifTeleporting)
        {
            Vector3 playerPosition = playerTransform.position;
            Vector3 playerUpPosition = playerPosition + new Vector3(0, 0.4f, 0);
            Vector3 playerUpUpPosition = playerPosition + new Vector3(0, 0.8f, 0);
            Physics.Raycast(playerUpPosition, way, out hitUpSideInfo);
            if (!(hitUpSideInfo.collider != null && (hitUpSideInfo.point - playerUpPosition).magnitude < 0.3f))
            {
                Physics.Raycast(playerPosition, way, out hitSideInfo);
                if (hitSideInfo.collider != null && (hitSideInfo.point - playerPosition).magnitude < 0.3f)
                {
                    Physics.Raycast(playerUpUpPosition, way, out hitUpUpSideInfo);
                    if (hitSideInfo.collider.CompareTag("Ground") && !(hitUpUpSideInfo.collider != null && (hitUpUpSideInfo.point - playerUpUpPosition).magnitude < 0.3f))
                    {
                        StartCoroutine(climbIEnumerator(way));
                    }
                }
                else
                {
                    StartCoroutine(rotateIEnumerator(way));
                }
            }
        }
    }
    //爬一个高的坡
    private IEnumerator climbIEnumerator(Vector3 way)
    {
        ifMoving = true;
        playerRigidbody.constraints = RigidbodyConstraints.None;
        straighten();
        float tempInt = way.x + way.z;
        Vector3 lineAround = Vector3.Cross(way, Vector3.down);
        Vector3 linePoint = playerTransform.position + new Vector3(0.2f * tempInt, 0.2f, 0.2f * tempInt);
        for (int i = 0; i < 180 / rotateSpeed; i++)
        {
            playerTransform.RotateAround(linePoint, lineAround, rotateSpeed);
            yield return null;
        }
        straighten();
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        ifMoving = false;
    }
    //平地运动
    private IEnumerator rotateIEnumerator(Vector3 way)
    {
        ifMoving = true;
        playerRigidbody.constraints = RigidbodyConstraints.None;
        straighten();
        float tempInt = way.x + way.z;
        Vector3 lineAround = Vector3.Cross(way, Vector3.down);
        Vector3 linePoint = playerTransform.position + new Vector3(0.2f * tempInt, -0.2f, 0.2f * tempInt);
        for (int i = 0; i < 90 / rotateSpeed; i++)
        {
            playerTransform.RotateAround(linePoint, lineAround, rotateSpeed);
            yield return null;
        }
        straighten();
        yield return StartCoroutine(fallAfterRotateIEnumerator());
    }
    // 旋转完毕后进行下落，实时监测是否碰撞(接近)到地面
    private IEnumerator fallAfterRotateIEnumerator()
    {
        Physics.Raycast(playerTransform.position, gravityDirection, out hitBelowInfo);
        if (!(hitBelowInfo.collider != null && hitBelowInfo.collider.CompareTag("Ground") && (hitBelowInfo.point - playerTransform.position).magnitude < 0.3f))
        {
            playerRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        while (ifMoving && !ifTeleporting)
        {
            Physics.Raycast(playerTransform.position, gravityDirection, out hitBelowInfo);
            playerRigidbody.AddForce(gravityForce * rotateSpeed, ForceMode.Acceleration);
            if (hitBelowInfo.collider != null && hitBelowInfo.collider.CompareTag("Ground") && (hitBelowInfo.point - playerTransform.position).magnitude < 0.3f)
            {
                straighten();
                playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                ifMoving = false;
            }
            if (playerTransform.position.y < -10)
            {
                StartCoroutine(PlayerController.instance.restartIEnumerator());
            }
            yield return null;
        }
    }
    //到达目标点动画
    public IEnumerator winIEnumerator(Transform destination)
    {
        ifTeleporting = true;
        float speed = 1, bloomThreshold = 1.25f;
        bool tempbool = false;
        Vector3 rotatePoint = playerTransform.position;
        playerRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        while (speed < 10)
        {
            for (int i = 0; i < 180 / (speed * rotateSpeed); i++)
            {
                playerTransform.RotateAround(rotatePoint, -gravityDirection, speed * rotateSpeed);
                if (!tempbool && playerRigidbody.velocity.magnitude < 3)
                {
                    playerRigidbody.AddForce(-0.5f * gravityDirection, ForceMode.Acceleration);
                }
                else
                {
                    if (!tempbool)
                    {
                        tempbool = true;
                    }
                    playerRigidbody.AddForce(gravityDirection * 0.2f, ForceMode.Acceleration);
                }
                if (tempbool && playerRigidbody.velocity.magnitude < 0.1f)
                {
                    playerRigidbody.constraints = RigidbodyConstraints.FreezePosition;
                }
                playerTransform.localScale += new Vector3(0.001f, 0.001f, 0.001f) * rotateSpeed;
                bloomThreshold -= 0.001f * rotateSpeed;
                VolumeController.instance.changeBloomThreshold(bloomThreshold);
                yield return null;
            }
            speed += 0.25f;
        }
        Transform goalTransformOld = goalTransform;
        levelIndex = levelIndex % levelTransform.childCount + 1;
        levelNowTransform = levelTransform.GetChild(levelIndex - 1);
        startTransform = levelNowTransform.Find("Start");
        goalTransform = levelNowTransform.Find("Goal");
        goalTransform.gameObject.SetActive(true);
        playerTransform.localScale = Vector3.one;
        playerTransform.position = startTransform.position;
        playerTransform.rotation = startTransform.rotation;
        straighten();
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        while (bloomThreshold < 1.25f)
        {
            bloomThreshold += 0.005f * rotateSpeed;
            VolumeController.instance.changeBloomThreshold(bloomThreshold);
            yield return null;
        }
        ifTeleporting = false;
        goalTransformOld.gameObject.SetActive(false);
    }
    //本关重开动画
    private IEnumerator restartIEnumerator()
    {
        ifTeleporting = true;
        float bloomThreshold = 1.25f;
        playerRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        for (int i = 0; i < 1000; i++)
        {
            playerTransform.localScale += new Vector3(0.000001f, 0.000001f, 0.000001f) * i * rotateSpeed;
            bloomThreshold -= 0.001f * rotateSpeed;
            VolumeController.instance.changeBloomThreshold(bloomThreshold);
            yield return null;
        }
        playerTransform.localScale = Vector3.one;
        playerTransform.position = startTransform.position;
        playerTransform.rotation = startTransform.rotation;
        straighten();
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        while (bloomThreshold < 1.25f)
        {
            bloomThreshold += 0.005f * rotateSpeed;
            VolumeController.instance.changeBloomThreshold(bloomThreshold);
            yield return null;
        }
        ifTeleporting = false;
        ifMoving = false;
    }

    public void restart()
    {
        StartCoroutine(PlayerController.instance.restartIEnumerator());
    }
    #endregion
}