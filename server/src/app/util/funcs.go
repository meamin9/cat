package util

import "regexp"

// 是否由数字字母下划线短划线组成
func IsWords(str string) bool {
	ok, _ := regexp.MatchString("[0-9a-zA-Z_-]+", str)
	return ok
}
