using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private Transform levelsTransform, goalTransform, resetTransform;
    public static LevelController instance;
    private void Awake(){
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        levelsTransform = GameObject.Find("Levels").GetComponent<Transform>();
    }

    private void Start(){
        showSingleLevel(1);
    }
    
    public void showAllLevel()
    {
        int levelCount = levelsTransform.childCount;
        for(int i = 0; i < levelCount; i++){
            Transform singleLevelTransform = levelsTransform.GetChild(i);
            singleLevelTransform.gameObject.SetActive(true);
        }
    }

    public void showSingleLevel(int levelIndex)
    {
        int levelCount = levelsTransform.childCount;
        for(int i = 0; i < levelCount; i++){
            Transform singleLevelTransform = levelsTransform.GetChild(i);
            singleLevelTransform.gameObject.SetActive(false);
        }
        setAllStartInactive();
        Transform showedLevelTransform = levelsTransform.GetChild(levelIndex);
        showedLevelTransform.gameObject.SetActive(true);
        Transform goalTransform = showedLevelTransform.Find("Goal");
        goalTransform.gameObject.SetActive(true);
    }

    public Transform getResetTransform(){
        return resetTransform;
    }

    public void setResetTransform(Transform resetTransform){
        this.resetTransform = resetTransform;
    }

    public void setAllGoalInactive(){
        Debug.Log("setAllGoalInactive");
        int levelCount = levelsTransform.childCount;
        for(int i = 1; i < levelCount; i++){
            Transform singleLevelTransform = levelsTransform.GetChild(i);
            Transform goalTransform = singleLevelTransform.Find("Goal");
            goalTransform.gameObject.SetActive(false);
        }
    }

    public void setAllStartInactive(){
        int levelCount = levelsTransform.childCount;
        for(int i = 1; i < levelCount; i++){
            Transform singleLevelTransform = levelsTransform.GetChild(i);
            Transform startTransform = singleLevelTransform.Find("Start");
            startTransform.gameObject.SetActive(false);
        }
    }

    public void setAllStartActive(){
        int levelCount = levelsTransform.childCount;
        for(int i = 1; i < levelCount; i++){
            Transform singleLevelTransform = levelsTransform.GetChild(i);
            Transform startTransform = singleLevelTransform.Find("Start");
            startTransform.gameObject.SetActive(true);
        }
    }
}
