// Released to the public domain. Use, modify and relicense at will.
//#r "references/OpenTK.dll"

open Utils

open System
open System.Drawing
open System.Collections.Generic

open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL
open OpenTK.Input
open OpenTK.Windowing.Desktop
open OpenTK.Windowing.Common
open OpenTK.Windowing.GraphicsLibraryFramework
open OpenTK.Mathematics

open Shader
open PTHexGrid

let nativeWindowSettings = NativeWindowSettings()
nativeWindowSettings.Size <- Vector2i(800, 600)
nativeWindowSettings.Title <- "Blarg"

type Game() =
    inherit GameWindow(GameWindowSettings.Default, nativeWindowSettings)

    let mutable VertexBufferObject = Unchecked.defaultof<int>
    let mutable VertexArrayObject = Unchecked.defaultof<int>
    // let mutable ElementBufferObject = Unchecked.defaultof<int>

    member this.Shader = Shader("../../../shader.vert", "../../../shader.frag")

    override o.OnKeyDown e =
        if e.Key = Keys.Escape then
            do base.Close()

        do base.OnKeyDown e

    override o.OnLoad() =
        o.Shader.init()
        
        VertexBufferObject <- GL.GenBuffer()
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject)

        // ElementBufferObject <- GL.GenBuffer()
        // GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject)

        let hexVertices : float32 [] =
            [| 0.5f; -0.5f; 0.0f //Bottom-left vertex
               0.5f; -0.5f; 0.0f //Bottom-right vertex
               0.0f;  0.5f; 0.0f
            |]
            // hexUnitVertices
            // |> Array.map (fun v -> [| v.X; v.Y; 0f |])
            // |> Array.concat
            // |> Array.map ((*) 10f)
        
        GL.BufferData(BufferTarget.ArrayBuffer, hexVertices.Length * sizeof<float32>, hexVertices, BufferUsageHint.StaticDraw)

        VertexArrayObject <- GL.GenVertexArray()
        GL.BindVertexArray(VertexArrayObject)

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof<float32>, 0)
        GL.EnableVertexAttribArray(0)

        // let indices : uint [] =
        //     [|0u; 1u; 2u
        //       0u; 2u; 3u
        //       0u; 3u; 4u
        //       0u; 4u; 5u
        //     |]
        
        // GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof<uint>, indices, BufferUsageHint.StaticDraw)

        do GL.ClearColor(Color4(1f, 0f, 0f, 0f))
        do base.OnLoad()

    override o.OnRenderFrame e =
        do GL.Clear(ClearBufferMask.ColorBufferBit)

        o.Shader.Use()

        GL.BindVertexArray(VertexArrayObject)

        GL.DrawArrays(PrimitiveType.Triangles, 0, 3)

        do base.SwapBuffers()

        do base.OnRenderFrame e


// type Game() =
//     /// <summary>Creates a 800x600 window with the specified title.</summary>
//     inherit GameWindow(800, 600, GraphicsMode.Default, "F# OpenTK Sample")

//      do base.VSync <- VSyncMode.On

//      /// <summary>Load resources here.</summary>
//      /// <param name="e">Not used.</param>

//      override o.OnLoad e =
//        base.OnLoad(e)
//        GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f)
//        GL.Enable(EnableCap.DepthTest)

//      /// <summary>
//      /// Called when your window is resized. Set your viewport here. It is also
//      /// a good place to set up your projection matrix (which probably changes
//      /// along when the aspect ratio of your window).
//      /// </summary>
//      /// <param name="e">Not used.</param>
//      override o.OnResize e =
//          base.OnResize e
//          GL.Viewport(base.ClientRectangle.X, base.ClientRectangle.Y, base.ClientRectangle.Width, base.ClientRectangle.Height)
//          let mutable projection = Matrix4.CreatePerspectiveFieldOfView(float32 (Math.PI / 4.), float32 base.Width / float32 base.Height, 1.f, 64.f)
//          GL.MatrixMode(MatrixMode.Projection)
//          GL.LoadMatrix(&projection)


//      /// <summary>
//      /// Called when it is time to setup the next frame. Add you game logic here.
//      /// </summary>
//      /// <param name="e">Contains timing information for framerate independent logic.</param>
//      override o.OnUpdateFrame e =
//        base.OnUpdateFrame e
//       //  if base.Keyboard.[Key.Escape] then base.Close()

//      /// <summary>
//      /// Called when it is time to render the next frame. Add your rendering code here.
//      /// </summary>
//      /// <param name="e">Contains timing information.</param>
//      override o.OnRenderFrame(e) =
//        base.OnRenderFrame e
//        GL.Clear(ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit)
//        let mutable modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY)
//        GL.MatrixMode(MatrixMode.Modelview)
//        GL.LoadMatrix(&modelview)

//        GL.Begin(BeginMode.Triangles)
//        GL.Color3(1.f, 1.f, 0.f); GL.Vertex3(-1.f, -1.f, 4.f)
//        GL.Color3(1.f, 0.f, 0.f); GL.Vertex3(1.f, -1.f, 4.f)
//        GL.Color3(0.2f, 0.9f, 1.f); GL.Vertex3(0.f, 1.f, 4.f)
//        GL.End()

//        base.SwapBuffers()

// /// <summary>
// /// The main entry point for the application.
// /// </summary>
let game = new Game()

do game.Run()
