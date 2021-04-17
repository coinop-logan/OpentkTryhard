module Shader

open System
open System.IO
open System.Text
open System.Collections.Generic
open OpenTK.Graphics.OpenGL4
open OpenTK.Mathematics
open OpenTK.Graphics

type Shader(vertPath, fragPath) =
    let mutable Handle = Unchecked.defaultof<int>

    member this.init() =
        Handle <- GL.CreateProgram()
        printf "hello"

        let vertSource = File.ReadAllText(vertPath)
        let vertShader = GL.CreateShader(ShaderType.VertexShader)
        do GL.ShaderSource(vertShader, vertSource)
        do GL.CompileShader(vertShader)

        let fragSource = File.ReadAllText(fragPath)
        let fragShader = GL.CreateShader(ShaderType.FragmentShader);
        do GL.ShaderSource(fragShader, fragSource)
        do GL.CompileShader(fragShader)
        
        do GL.AttachShader(Handle, vertShader)
        do GL.AttachShader(Handle, fragShader)

        do GL.LinkProgram(Handle)

        do GL.DetachShader(Handle, vertShader)
        do GL.DetachShader(Handle, fragShader)
        do GL.DeleteShader(fragShader)
        do GL.DeleteShader(vertShader)

        // let numUniforms = ref 0
        // do GL.GetProgram(this.Handle, GetProgramParameterName.ActiveUniforms, numUniforms);

    // member this.uniformLocations =
    //     do printf "2"
    //     List.init !numUniforms
    //         (fun i ->
    //             let key = GL.GetActiveUniform(_Handle, i, ref 0, ref Unchecked.defaultof<ActiveUniformType>);
    //             let location = GL.GetUniformLocation(_Handle, key);
    //             (key, location)
    //         )
    //         |> Map.ofList
    
    member this.Use() =
        do GL.UseProgram(Handle)