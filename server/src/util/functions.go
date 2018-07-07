package util

import (
	"os"
	"encoding/csv"
	"reflect"
	"strconv"
	"log"
	"errors"
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

	for j, line := range lines {
		vp := reflect.New(structType)
		v := vp.Elem()
		for i, str := range line {
			log.Print(names[i])
			f := v.FieldByName(names[i])
			if !f.IsValid() {
				log.Print("未使用的字段", filepath, names[i], v.Type())
				continue
			}
			if ParseStr(str, f.Addr().Interface()) != nil {
				panic(err)
			}
		}
		log.Print(v.Interface(), vp.Interface())
		vlist.Index(j).Set(vp)
		log.Print(vlist.Interface())
	}
	reflect.ValueOf(data).Elem().Set(vlist)
}


var (
	strconvMap = make(map[reflect.Type]func(str string, vptr interface{}) error)
)

func ParseStr(str string, vptr interface{}) error {
	if p, ok := strconvMap[reflect.TypeOf(vptr)]; ok {
		return p(str, vptr)
	}
	return errors.New("meiyou found 解析方法")
}

func RegStrconvParser(vptr interface{}, parser func(str string, vptr interface{}) error) {
	typ := reflect.TypeOf(vptr)
	if _, ok := strconvMap[typ]; ok {
		panic("文本解析器被覆盖")
	}
	strconvMap[typ] = parser
}

func regDefaultParser() {
	//string
	RegStrconvParser((*string)(nil), func(str string, vptr interface{}) error{
		*vptr.(*string) = str
		return nil
	})

	// int
	RegStrconvParser((*int)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseInt(str, 0, 0)
		*vptr.(*int) = int(v)
		return err
	})
	RegStrconvParser((*int8)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseInt(str, 0, 8)
		*vptr.(*int8) = int8(v)
		return err
	})
	RegStrconvParser((*int16)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseInt(str, 0, 16)
		*vptr.(*int16) = int16(v)
		return err
	})
	RegStrconvParser((*int32)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseInt(str, 0, 32)
		*vptr.(*int32) = int32(v)
		return err
	})
	RegStrconvParser((*int64)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseInt(str, 0, 64)
		*vptr.(*int64) = int64(v)
		return err
	})

	// uint
	RegStrconvParser((*uint)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseUint(str, 0, 0)
		*vptr.(*uint) = uint(v)
		return err
	})
	RegStrconvParser((*uint8)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseUint(str, 0, 8)
		*vptr.(*uint8) = uint8(v)
		return err
	})
	RegStrconvParser((*uint16)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseUint(str, 0, 16)
		*vptr.(*uint16) = uint16(v)
		return err
	})
	RegStrconvParser((*uint32)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseUint(str, 0, 32)
		*vptr.(*uint32) = uint32(v)
		return err
	})
	RegStrconvParser((*uint64)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseUint(str, 0, 64)
		*vptr.(*uint64) = uint64(v)
		return err
	})

	//float
	RegStrconvParser((*float32)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseFloat(str, 32)
		*vptr.(*float32) = float32(v)
		return err
	})
	RegStrconvParser((*float64)(nil), func(str string, vptr interface{}) error{
		v, err := strconv.ParseFloat(str, 64)
		*vptr.(*float64) = float64(v)
		return err
	})
}
//
//// 整数要求是10进制
//func paserString2(str string, vptr interface{}) error {
//	var err error
//	switch p := vptr.(type) {
//	case *string:
//		*p, err = str, nil
//	case *int:
//		*p, err = strconv.Atoi(str)
//	case *int8:
//		v, e := strconv.Atoi(str)
//		*p, err = int8(v), e
//	case *int16:
//		v, e := strconv.Atoi(str)
//		*p, err = int16(v), e
//	case *int32:
//		v, e := strconv.Atoi(str)
//		*p, err = int32(v), e
//	case *int64:
//		v, e := strconv.Atoi(str)
//		*p, err = int64(v), e
//	case *proto.SCNotice:
//		i, e := strconv.ParseUint(str, 10, 0)
//		v, err = uint(i), e
//	case reflect.Uint8:
//		i, e := strconv.ParseUint(str, 10, 0)
//		v, err = uint(i), e
//	case reflect.Uint16:
//		fallthrough
//	case reflect.Uint32:
//		fallthrough
//	case reflect.Uint64:
//
//
//	}
//	return v, err
//}

func init() {
	regDefaultParser()
}