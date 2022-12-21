using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject objectInHand;
    [SerializeField] private Transform objectInHandPos;
    [SerializeField] private Transform spherStartPoint;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private AudioSource sound1;
    [SerializeField] private AudioSource sound2;
    [SerializeField] private TextMeshProUGUI toastMessage;

    private FirstPersonController _fPController;

    private void Start()
    {
        _fPController = FindObjectOfType<FirstPersonController>();
    }
    private void OnTriggerEnter(Collider other)
    {           
        if (!objectInHand && other.gameObject.CompareTag("Sphere"))
        {
            StartCoroutine(StopMovement(1f));
            sound2.Play();
            PickupSphere(other.gameObject);
        }

        if (objectInHand && other.gameObject.CompareTag("Box"))
        {
            StartCoroutine(StopMovement(1f));
            sound1.Play();
            DropSphere(other.gameObject);

        }
    }

    private void DropSphere(GameObject box)
    {
        GameObject _sphere = objectInHand;
        objectInHand = null;
        _sphere.transform.SetParent(box.transform, true);
        _sphere.transform.DOLocalMove(box.transform.GetChild(0).localPosition, 1).OnComplete(() =>
        {
            _sphere.transform.parent = null;
            _sphere.transform.SetPositionAndRotation(spherStartPoint.position, spherStartPoint.rotation);
            _sphere.transform.localScale = spherStartPoint.localScale;
            _sphere.transform.GetChild(0).gameObject.SetActive(true);


            box.GetComponentInChildren<ParticleSystem>().Play(true);

            switch (box.name)
            {
                case "BoxA":
                    StartCoroutine(ShowToast("you have dropped in box A", 2f));
                    break;
                case "BoxB":
                    StartCoroutine(ShowToast("you have dropped in box B", 2f));
                    break;
                case "BoxC":
                    StartCoroutine(MoveToStartPos(2f));
                    break;

            }

            _sphere.GetComponent<BoxCollider>().enabled = true;


        });
    }

    private void PickupSphere(GameObject sphere)
    {
        objectInHand = sphere;
        objectInHand.GetComponent<BoxCollider>().enabled = false;
        objectInHand.transform.GetChild(0).gameObject.SetActive(false);
        objectInHand.transform.SetParent(objectInHandPos.parent, true);
        objectInHand.transform.DORotate(objectInHandPos.rotation.eulerAngles, 0.5f);
        objectInHand.transform.DOScale(objectInHandPos.localScale, 0.5f);
        objectInHand.transform.DOLocalMove(objectInHandPos.localPosition, 0.5f).OnComplete(() =>
        {
            uiPanel.SetActive(true);
        });
    }

    IEnumerator MoveToStartPos(float time)
    {
        yield return new WaitForSeconds(time);
        transform.parent.position = spherStartPoint.position;
        transform.parent.rotation = Quaternion.Euler(new Vector3(0, 20, 0));
    }

    IEnumerator StopMovement(float time)
    {
        _fPController.canMove = false;
        yield return new WaitForSeconds(time);
        _fPController.canMove = true;

    }
    IEnumerator ShowToast(string message, float time)
    {
        toastMessage.text = message;
        toastMessage.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        toastMessage.transform.parent.gameObject.SetActive(false);
    }
}
    
