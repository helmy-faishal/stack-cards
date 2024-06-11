using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PackCard : Card
{
    [Tooltip("Effect saat membuka kartu")]
    [SerializeField] ParticleSystem openEffect;

    [Tooltip("Waktu yang dibutuhkan untuk membuka kartu")]
    [SerializeField] float openDelay = 2f;

    [Tooltip("Prefab kartu yang akan dikeluarkan dari Pack")]
    [SerializeField] GameObject[] packContents;

    [Tooltip("Jumlah dari prefab yang dikeluarkan")]
    [SerializeField] [Min(1)] int contentOutput = 1;

    bool isOpeningPack;
    protected override void SetStart()
    {
        base.SetStart();
        if (gameManager != null)
        {
            gameManager.control.OnCardPressed += OpenPack;
        }

        openEffect.Stop();
    }

    protected override void SetOnDestroy()
    {
        base.SetOnDestroy();
        if (gameManager != null )
        {
            gameManager.control.OnCardPressed -= OpenPack;
        }

        gameManager.TotalPackCard -= 1;
    }

    void OpenPack(Card card)
    {
        if (card != this) return;
        if (isOpeningPack) return;
        if (contentOutput <= 0) return;

        StartCoroutine(OpenPackCoroutine());
    }

    IEnumerator OpenPackCoroutine()
    {
        isOpeningPack = true;
        openEffect.Play();

        yield return new WaitForSeconds(openDelay);
        for (int i = 0; i < contentOutput; i++)
        {
            Vector3 position = GameUtility.GetRandomScatterPosition(transform.position, 2f);
            Instantiate(GetRandomContent(), position, Quaternion.identity);
        }
        openEffect.Stop();
        DestroyCard();
    }

    GameObject GetRandomContent()
    {
        return packContents[Random.Range(0, packContents.Length)];
    }
}
