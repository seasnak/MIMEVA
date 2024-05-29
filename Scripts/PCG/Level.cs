using System;
using System.Collections.Generic;

using Godot;

namespace Mimeva;

public partial class Level {

    private List<List<string>> level_mat;
    private int in_pos;
    private int out_pos;
    private int in_size;
    private int out_size;

    public Level(ref List<List<string>> _level_mat, int _in_pos, int _out_pos, int _in_size, int _out_size) {
        
        in_pos = _in_pos;
        out_pos = _out_pos;

        in_size = _in_size;
        out_size = _out_size;

        // deep copy level matrix
        for(int i = 0; i < _level_mat.Count; i++) {
            for(int j = 0; j < _level_mat[0].Count; j++) {
                level_mat[i][j] = _level_mat[i][j];
            }
        }
        
    }



}