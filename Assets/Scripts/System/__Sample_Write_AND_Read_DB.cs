using UnityEngine;
using System.Collections;

using File_IO;

public class __Sample_Write_AND_Read_DB : MonoBehaviour {

	// Use this for initialization
	void Start () {
        IO_SqlDB.WriteDB_CUR("behaveData_SAMPLE", "WritingSample", new Vector3(-12.3f, 4.5f, -6.78f), 43f, 21f, new Vector3(6,-5,4), 3, true, false);
        IO_SqlDB.WriteDB_AFT("behaveData_SAMPLE", "WritingSample", new Vector3(-87.6f, 5.4f, -3.21f), 12f, 34f, new Vector3(-6, 5, -4), 2, false, true);
    }
	
}
