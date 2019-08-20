package consts


const (
	OK = iota
	ErrAccountIdLen // 帐号长度在{0}到{1}之间
	ErrAccountId // 帐号Id含有非法字符
	ErrAccountPwdLen // 密码长度在{0}到{1}之间
	ErrAccountExist // 帐号已经存在

	ErrRoleLogin

	ErrRoleNameInvalid
	ErrRoleNameLen
	ErrRoleNameExist


)
