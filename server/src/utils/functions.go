package utils

func ResetCapcity(sl []interface{}, capacity int) (newsl []interface{}) {
	if cap(sl) == capacity {
		return sl
	}
	newsl = make([]interface{}, capacity)
	copy(newsl, sl)
	return
}
