﻿using System.Numerics;


namespace OpenGlTIPE.Rendering.shapes
{
    class VertexShapes
    {
        private Vector2 position;
        private Vector2 scale;
        private float rotation;

        private float[] vertices;
        private float[] colors;

        private Matrix4x4 ModelMatrix;
        // uint vertsId = glGenBuffer();
        //uint colsId = glGenBuffer();

/*
 * glBindBuffer(GL_ARRAY_BUFFER, vertsId);
    fixed (float* v = &vertices[0]) {
        glBufferData(GL_ARRAY_BUFFER, vertices.Length * sizeof(float), v, GL_STATIC_DRAW);
    }
    // format of the data
    // index of buffer : 0 | first List of 2 elements | of type float | not normalised |
    // the next paire is 5 away | offset is 0 
    glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(float) * 2, (void*)0);
    glEnableVertexAttribArray(0);



    glBindBuffer(GL_ARRAY_BUFFER, colsId);
    fixed (float* v = &colors[0])
    {
        glBufferData(GL_ARRAY_BUFFER, colors.Length * sizeof(float), v, GL_STATIC_DRAW);
    }
    // format of the data
    // index of buffer : 0 | first List of 2 elements | of type float | not normalised |
    // the next paire is 5 away | offset is 0 
    glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(float) * 3, (void*)0);
    glEnableVertexAttribArray(1);
*/

public VertexShapes(Vector2 position, Vector2 scale, float rotation, float[] vertices, float[] colors)
{
    this.position = position;
    this.scale = scale;
    this.rotation = rotation;
    this.vertices = vertices;
    this.colors = colors;

    CalculateModelMatrix();
}


public Vector2 GetPosition() => position;
public Vector2 GetScale() => scale;
public float GetRotation() => rotation;
public float[] GetVertices() => vertices;
public float[] GetColors() => colors;
public Matrix4x4 GetModelMatrix() => ModelMatrix;

private void CalculateModelMatrix()
{
    Matrix4x4 translationMat = Matrix4x4.CreateTranslation(position.X, position.Y, 0);
    Matrix4x4 scaleMat = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
    Matrix4x4 rotationMat = Matrix4x4.CreateRotationZ(rotation);
    ModelMatrix = scaleMat * rotationMat * translationMat;
}

// vao , vbo
// public void draw () {}

}
}
