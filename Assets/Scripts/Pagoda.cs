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
}