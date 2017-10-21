package main

const codeTemplate = `// Generated by cellnet/objprotogen
// DO NOT EDIT!
package {{.PackageName}}

{{if gt (.Structs|len) 0}}
import (
	"cellnet"
	"reflect"
	"fmt"
)
{{end}}

{{range .Structs}}
func (self *{{.Name}}) String() string { return fmt.Sprintf("%+v",*self) } {{end}}

func init() {
	{{range .Structs}}
	cellnet.RegisterMessageMeta("binary","{{$.PackageName}}.{{.Name}}", reflect.TypeOf((*{{.Name}})(nil)).Elem(), {{.MsgID}})	{{end}}
}

`

func genCode(output string, f *Package) {

	generateCode("objprotogen", codeTemplate, output, f, &generateOption{formatGoCode: true})
}