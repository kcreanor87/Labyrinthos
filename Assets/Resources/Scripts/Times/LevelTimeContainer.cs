using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimeContainer : MonoBehaviour {

	public List <LevelTime> _levelTimes = new List <LevelTime>();

    public void Awake()
    {
        CheckTimes();
    }

    void CheckTimes()
    {
        for (int i = 0; i < _playerManager._totalLevels; i++)
        {
            if (_playerManager._times[i] <= _levelTimes[i]._S_time)
            {
                _levelTimes[i]._rank = "S";
            }
            else if (_playerManager._times[i] <= _levelTimes[i]._A_time)
            {
                _levelTimes[i]._rank = "A";
            }
            else if (_playerManager._times[i] <= _levelTimes[i]._B_time)
            {
                _levelTimes[i]._rank = "B";
            }
            else if (_playerManager._times[i] <= _levelTimes[i]._C_time)
            {
                _levelTimes[i]._rank = "C";
            }
            else
            {
                _levelTimes[i]._rank = "-";
            }
        }
    }
}
