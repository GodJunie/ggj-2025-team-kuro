using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ
{
    public class BGImageController : MonoBehaviour
    {
        [SerializeField] List<GameObject> bgObjects;

        [SerializeField] float xDistance = 10;
        [SerializeField] float yDistance = 17.8f;
        [SerializeField] float xAdjust = 2;
        [SerializeField] float yAdjust = 2f;

        [SerializeField] private PlayerController player;

        void Update()
        {
            if (player.IsDead) return;

            AdjustXaxis();
            AdjustYaxis();
        }

        private void AdjustXaxis()
        {
            float playerX = player.transform.position.x;

            foreach (var image in bgObjects)
            {
                float imageX = image.transform.position.x;
                float imageY = image.transform.position.y;
                if (Mathf.Abs(playerX - imageX) >= (xDistance * xAdjust))
                {
                    float targetX = playerX > imageX ?
                    imageX + xDistance * 7 //move to right
                : imageX - xDistance * 7;  //move to left

                    image.transform.position = new Vector3(targetX, imageY, 0f);
                }
            }
        }

        private void AdjustYaxis()
        {
            float playerY = player.transform.position.y;

            foreach (var image in bgObjects)
            {
                float imageX = image.transform.position.x;
                float imageY = image.transform.position.y;
                if (Mathf.Abs(playerY - imageY) >= (yDistance * yAdjust))
                {
                    float targetY = playerY > imageY ?
                    imageY + yDistance * 4 //move to down
                : imageY - yDistance * 4;  //move to up

                    image.transform.position = new Vector3(imageX, targetY, 0f);
                }
            }
        }
    }


}
