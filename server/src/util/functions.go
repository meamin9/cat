package util

import (
	"os"
	"encoding/csv"
	"reflect"
	"strconv"
	"log"
)

func ResetCapcity(sl []interface{}, capacity int) (newsl []interface{}) {
	if cap(sl) == capacity {
		return sl
	}
	newsl = make([]interface{}, capacity)
	copy(newsl, sl)
	return
}

//linedesc 是一个结构体指针,可以是nil，用来描述一行的信息
// data的类型要求是 *[]*struct， struct描述了csv的一行
func LoadCsv(filepath string, data interface{}){
	f, err := os.Open(filepath)
	if err != nil {
		panic(err)
	}
	defer f.Close()
	lines, err := csv.NewReader(f).ReadAll()
	if err != nil {
		panic(err)
	}
	// 第一行是注释, 第二行是字段名
	n := len(lines)
	names := lines[1]
	lines = lines[2:]

	slicetType := reflect.TypeOf(data).Elem() // []*struct
	structType := slicetType.Elem().Elem() // struct
	vlist := reflect.MakeSlice(slicetType, n-2, n-2)

	for _, line := range lines {
		vp := reflect.New(structType)
		v := vp.Elem()
		//log.Print(vp, v)
		for i, str := range line {
			log.Print(names[i])
			f := v.FieldByName(names[i])
			if !f.IsValid() {
				log.Print("未使用的字段", filepath, names[i], v.Type())
				continue
			}
			strv, err := paserString(f.Type(), str)
			if err != nil {
				panic(err)
			}
			f.Set(reflect.ValueOf(strv))
		}
		reflect.Append(vlist, vp)
	}
	reflect.ValueOf(data).Elem().Set(vlist)

}

func paserString(typ reflect.Type, str string) (v interface{}, err error) {
	switch typ.Kind() {
	case reflect.String:
		v, err = str, nil
	case reflect.Int:
		i, e := strconv.Atoi(str)
		v, err = int(i), e
	case reflect.Int8:
		i, e := strconv.Atoi(str)
		v, err = int8(i), e
	case reflect.Int16:
		i, e := strconv.Atoi(str)
		v, err = int16(i), e
	case reflect.Int32:
		i, e := strconv.Atoi(str)
		v, err = int32(i), e
	case reflect.Int64:
		v, err = strconv.Atoi(str)
	case reflect.Uint:
		i, e := strconv.ParseUint(str, 10, 0)
		v, err = uint(i), e
	case reflect.Uint8:
		i, e := strconv.ParseUint(str, 10, 0)
		v, err = uint(i), e
	case reflect.Uint16:
		fallthrough
	case reflect.Uint32:
		fallthrough
	case reflect.Uint64:


	}
	return v, err
}