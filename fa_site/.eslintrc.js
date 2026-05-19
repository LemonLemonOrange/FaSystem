module.exports = {
	extends: ["react-app", "react-app/jest"],
	rules: {
		"no-console": ["warn", { allow: ["warn", "error"] }],
		"no-unused-vars": ["warn", { argsIgnorePattern: "^_", varsIgnorePattern: "^_" }],
		eqeqeq: ["error", "always"],
		"no-duplicate-imports": "error",
		"react-hooks/exhaustive-deps": "warn",
	},
};