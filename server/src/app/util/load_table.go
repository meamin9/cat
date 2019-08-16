package util

import (
	"encoding/csv"
	"encoding/json"
	"errors"
	"os"
	"reflect"
	"strconv"
)

// 打印错误，外面不要检测错误了
func LoadJson(path string, result interface{}) {
	f, err := os.Open(path)
	if err == nil {
		defer f.Close()
		size, _ := f.Seek(0, 2)
		b := make([]byte, size)
		f.Seek(0, 0)
		f.Read(b)
		if err = json.Unmarshal(b, result); err != nil {
			log.Errorf("load json(%v) error: %v", path, err)
		}
	} else {
		log.Errorf("open json(%v) error: %v", path, err)
	}
}


// 按行解析csv，第一行默认是注释，所以跳过。
// v是指向一个切片指针,切片元素是一个结构体的空指针: *[]*struct
func LoadCsv(path string, v interface{}) {
	lines := readCsv(path)
	// 第一行是注释, 第二行是字段名
	n := len(lines)
	if n <= 2 {
		return
	}
	names := lines[1]
	lines = lines[2:]
	count := n - 2

	sliceT := reflect.TypeOf(v).Elem() // []*struct
	elementT := sliceT.Elem().Elem()    // struct
	sliceV := reflect.MakeSlice(elementT, count, count)
	for i, line := range lines {
		elementPtr := reflect.New(elementT)
		element := elementPtr.Elem()
		for i, str := range line {
			f := element.FieldByName(names[i])
			if !f.IsValid() {
				log.Warnf("Not Found field:%v in File: %v", names[i], path)
				continue
			}
			if ParseStr(str, f.Addr().Interface()) != nil {
				log.Errorf("parse filed error: %v in File:%v", names[i], path)
			}
		}
		sliceV.Index(i).Set(elementPtr)
	}
	reflect.ValueOf(v).Elem().Set(sliceV)
}

// 按列解析CSV，第1行是注释，只解析一二列
// v是指向一个结构体的空指针
func LoadCsvTranspose(path string, v interface{}) {
	lines := readCsv(path)
	n := len(lines)
	if n <= 2 {
		return
	}
	lines = lines[2:]
	vT := reflect.TypeOf(v).Elem() // strcut
	data := reflect.New(vT)
	for _, line := range lines {
		name, value := line[0], line[1]
		f := data.FieldByName(name)
		if !f.IsValid() {
			log.Warnf("Not Found field:%v in File: %v", name, path)
			continue
		}
		if ParseStr(value, f.Addr().Interface()) != nil {
			log.Errorf("parse filed error: %v in File:%v", name, path)
		}
	}
	reflect.ValueOf(v).Elem().Set(data)
}

func readCsv(path string) [][]string {
	if f, err := os.Open(path); err == nil {
		defer f.Close()
		if lines, err := csv.NewReader(f).ReadAll(); err == nil {
			return lines
		}
	}
	log.Errorf("读取csv出错：%v", path)
	return nil
}

// region: 文本解析
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
		log.Errorf("文本解析器重复注册: %v", typ)
		return
	}
	strconvMap[typ] = parser
}

func regDefaultParser() {
	//string
	RegStrconvParser((*string)(nil), func(str string, vptr interface{}) error {
		*vptr.(*string) = str
		return nil
	})

	// int
	RegStrconvParser((*int)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseInt(str, 0, 0)
		*vptr.(*int) = int(v)
		return err
	})
	RegStrconvParser((*int8)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseInt(str, 0, 8)
		*vptr.(*int8) = int8(v)
		return err
	})
	RegStrconvParser((*int16)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseInt(str, 0, 16)
		*vptr.(*int16) = int16(v)
		return err
	})
	RegStrconvParser((*int32)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseInt(str, 0, 32)
		*vptr.(*int32) = int32(v)
		return err
	})
	RegStrconvParser((*int64)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseInt(str, 0, 64)
		*vptr.(*int64) = int64(v)
		return err
	})

	// uint
	RegStrconvParser((*uint)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseUint(str, 0, 0)
		*vptr.(*uint) = uint(v)
		return err
	})
	RegStrconvParser((*uint8)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseUint(str, 0, 8)
		*vptr.(*uint8) = uint8(v)
		return err
	})
	RegStrconvParser((*uint16)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseUint(str, 0, 16)
		*vptr.(*uint16) = uint16(v)
		return err
	})
	RegStrconvParser((*uint32)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseUint(str, 0, 32)
		*vptr.(*uint32) = uint32(v)
		return err
	})
	RegStrconvParser((*uint64)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseUint(str, 0, 64)
		*vptr.(*uint64) = uint64(v)
		return err
	})

	//float
	RegStrconvParser((*float32)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseFloat(str, 32)
		*vptr.(*float32) = float32(v)
		return err
	})
	RegStrconvParser((*float64)(nil), func(str string, vptr interface{}) error {
		v, err := strconv.ParseFloat(str, 64)
		*vptr.(*float64) = float64(v)
		return err
	})
}

func init() {
	regDefaultParser()
}
