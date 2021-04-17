module PTHexGrid

open Utils
open System
open OpenTK.Mathematics

let hexUnitVertices: Vector2 [] =
    [| 0 .. 6 |]
    |> Array.map
        ((fun i ->
            float32 (Math.PI/6.0) +
            (float32 (i) / 6.0f) * (2.0f * float32 (Math.PI))
         )
         >> (fun angle -> Vector2(cos (angle), sin (angle))))

type AxialCoord =
    {
        Q : int
        R : int
    }
    static member (+) (c1, c2) =
        {
            Q = c1.Q + c2.Q
            R = c1.R + c2.R
        }
    static member (-) (c1, c2) =
        {
            Q = c1.Q - c2.Q
            R = c1.R - c2.R
        }
    static member (*) ((c : AxialCoord), i) =
        {
            Q = c.Q * i
            R = c.R * i
        }

let QBasis = Vector2(sqrt(3f), 0f)
let RBasis = Vector2(sqrt(3f)/2f, 3f/2f)

let axialToVec axial : Vector2 =
    (QBasis * float32 axial.Q)
        + (RBasis * float32 axial.R)

type CubeCoord =
    {
        X : int
        Y : int
        Z : int
    }

let cubeDistance a b =
    (abs <| a.X - b.X)
    |> max (abs <| a.Y - b.Y)
    |> max (abs <| a.Z - b.Z)

let axialToCube axial : CubeCoord =
    {
        X = axial.Q
        Z = axial.R
        Y = 0 - axial.Q - axial.R
    }

let hexDistance a b =
    let ac = axialToCube a
    let bc = axialToCube b

    cubeDistance ac bc

type HexGrid<'Tile>(width, height, scale : float32, initFunc: int -> int -> 'Tile) =
    member this.Tiles =
        Array2D.init width height initFunc
    member this.Scale : float32 = scale
    member this.DoForTiles (func : AxialCoord -> 'Tile -> unit) : unit =
        for q in 0 .. this.Tiles.GetLength(0) - 1 do
            for r in 0 .. this.Tiles.GetLength(1) - 1 do
                func {Q = q; R = r} this.Tiles.[q,r]
    member this.AxialToReal2D axial : Vector2 =
        (axialToVec axial) * (this.Scale)
    member this.AxialToReal3D axial =
        let v2 = this.AxialToReal2D axial

        Vector3(v2.X, v2.Y, 0.0f)

type Direction =
    | Right
    | UpRight
    | UpLeft
    | Left
    | DownLeft
    | DownRight

type RotateDirection =
    | Clockwise
    | CounterClockwise

let rotated (rotateDir : RotateDirection) (dir: Direction) : Direction =
    match rotateDir with
    | Clockwise ->
        match dir with
            | Right -> DownRight
            | UpRight -> Right
            | UpLeft -> UpRight
            | Left -> UpLeft
            | DownLeft -> Left
            | DownRight -> DownLeft
    | CounterClockwise ->
        match dir with
            | Right -> UpRight
            | UpRight -> UpLeft
            | UpLeft -> Left
            | Left -> DownLeft
            | DownLeft -> DownRight
            | DownRight -> Right
        

let randDir =
    randInst<Direction>

let getPosAngle (vec : Vector2) : float =
    atan2 (float(vec.Y)) (float(vec.X))
        |> fun angle ->
            if angle < 0.0 then
                angle + 2.0 * Math.PI
            else
                angle

let piOver3 = Math.PI / 3.0

// let vecToDir vec : Direction =
//     this trick won't work with PTHexGrid

//     let angle = getPosAngle vec

//     let dirIndex : int = angle / piOver3 |> int

//     numberedInst<Direction>(dirIndex)

let dirToAngle dir : float =
    piOver3 * (float
        (match dir with
            | Right -> 0
            | UpRight -> 1
            | UpLeft -> 2
            | Left -> 3
            | DownLeft -> 4
            | DownRight -> 5
        )
    )
    
let angleToUnitVec (angle: float) : Vector2 =
    Vector2(
        cos(float32(angle)),
        sin(float32(angle))
    )        

// let axialToDir : AxialCoord -> Direction =
//     axialToVec >> vecToDir