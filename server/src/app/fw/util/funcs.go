package util

import "regexp"

// 是否由ascii数字字母_-.@组成
func IsAsciiName(str string) bool {
	ok, _ := regexp.MatchString(`^[\w-.@]+$`, str)
	return ok
}

// 是否由unicode字母、数字、标点、符号组成
func IsUnicodeName(str string) bool {
	ok, _ := regexp.MatchString(`^[\pL\pN\pP\pS]+$`, str)
	return ok
}
