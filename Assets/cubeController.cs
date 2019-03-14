using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeController : MonoBehaviour
{

    public List<orientation> cubeOrientations = new List<orientation>();
    int currentIndex = 0;
    bool canRotate = false;
    public float rotateSpeed;
    public iTween.EaseType ease;
    public float dissolveSpeed;
    AudioSource audioSource;


    void Start()
    {

        audioSource = GetComponent<AudioSource>();

        //Setting all the gameobject's materials but the first one to zero
        for (int i = 1; i < cubeOrientations.Count; i++)
        {
            foreach(Renderer renderer in cubeOrientations[i].targetGameobject.GetComponentsInChildren<Renderer>())
            {
                foreach(Material mat in renderer.materials)
                {
                    if (mat.HasProperty("_fadePercent")) mat.SetFloat("_fadePercent", 1);
                    else if (mat.HasProperty("_opacity")) mat.SetFloat("_opacity", 0);
                    else mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0);
                }
            }
            cubeOrientations[i].targetGameobject.GetComponentInChildren<CanvasGroup>().alpha = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (canRotate && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)))
        {
            StartRotation(1);
        }
        else if (canRotate && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)))
        {
            StartRotation(-1);
        }
    }

    void StartRotation(int deltaIndex)
    {
        StartCoroutine(fadeOutAndRotate(deltaIndex));
    }

    void EndRotation()
    {
        StartCoroutine("fadeInAndEnableTurn");
    }

    void rotate(int deltaIndex)
    {
        currentIndex+=deltaIndex;
        if (currentIndex == cubeOrientations.Count) currentIndex = 0;
        if (currentIndex == -1) currentIndex = cubeOrientations.Count -1;

        iTween.RotateTo(gameObject, iTween.Hash("rotation", cubeOrientations[currentIndex].rotation,
                                                "easeType", ease,
                                                "speed", rotateSpeed,
                                                "onComplete", "EndRotation",
                                                "onCompleteTarget", gameObject));
    }

    IEnumerator fadeOutAndRotate(int deltaIndex)
    {
        playFadeOutSound();
        canRotate = false;
        float percentage = 0;
        while (percentage != 1)
        {
            yield return null; //Wait till next frame!
            percentage = Mathf.MoveTowards(percentage, 1, Time.deltaTime * dissolveSpeed);
            foreach (Renderer renderer in cubeOrientations[currentIndex].targetGameobject.GetComponentsInChildren<Renderer>())
            {
                foreach (Material mat in renderer.materials)
                {
                    if (mat.HasProperty("_fadePercent")) mat.SetFloat("_fadePercent", percentage);
                    else if (mat.HasProperty("_opacity")) mat.SetFloat("_opacity", 1 - percentage);
                    else mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1 - percentage);
                }
            }
            cubeOrientations[currentIndex].targetGameobject.GetComponentInChildren<CanvasGroup>().alpha = 1-percentage;
        }

        rotate(deltaIndex);
    }

    IEnumerator fadeInAndEnableTurn()
    {
        playFadeInSound();
        float percentage = 1;
        while (percentage != 0)
        {
            yield return null; //Wait till next frame!
            percentage = Mathf.MoveTowards(percentage, 0, Time.deltaTime * dissolveSpeed);
            foreach (Renderer renderer in cubeOrientations[currentIndex].targetGameobject.GetComponentsInChildren<Renderer>())
            {
                foreach (Material mat in renderer.materials)
                {
                    if (mat.HasProperty("_fadePercent")) mat.SetFloat("_fadePercent", percentage);
                    else if (mat.HasProperty("_opacity")) mat.SetFloat("_opacity", 1 - percentage);
                    else mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1 - percentage);
                }
            }
            cubeOrientations[currentIndex].targetGameobject.GetComponentInChildren<CanvasGroup>().alpha = 1 - percentage;
        }

        enableRotation();
    }

    public void enableRotation()
    {
        canRotate = true;
    }

    void playFadeInSound()
    {
        audioSource.pitch = 1;
        audioSource.time = 0;
        audioSource.Play();
    }

    void playFadeOutSound()
    {
        audioSource.time = audioSource.clip.length - .001f;
        audioSource.pitch = -1;
        audioSource.Play();
    }
}

[System.Serializable]
public class orientation
{
    public string name;
    public Vector3 rotation;
    public GameObject targetGameobject;
}
