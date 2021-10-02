using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pagoda : MonoBehaviour
{
    [SerializeField]
    private int width = 10;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private List<PagodaRow> rows;

    private float waveProgress = 0f;
    private float waveAmplitude = 1f;
    private float waveSpeed = .1f;

    private void Update()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            waveProgress += Time.deltaTime * waveSpeed;
            float offset = waveProgress + (i / waveAmplitude);
            rows[i].transform.position = new Vector3((Mathf.Sin(offset)) * waveAmplitude, rows[i].transform.position.y, rows[i].transform.position.z);

        }
    }
}