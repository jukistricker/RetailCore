import { useDispatch, useSelector } from "react-redux";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { loginUser } from "../../../store/thunks/accountThunk";
import { useNavigate, Link } from "react-router-dom";
import { LoginFormValues, loginSchema } from "../types";
import { ROUTES } from "../../../config/constants/url_routes";
import { InputField } from "../../../components/ui/InputField";
import { useEffect } from "react";
import { RootState } from "../../../store/store";

export const LoginForm = () => {
  const dispatch = useDispatch<any>();
  const navigate = useNavigate();
  const { loading, error, user } = useSelector(
    (state: RootState) => state.account,
  );

  useEffect(() => {
    if (user) {
      navigate(ROUTES.HOME);
    }
  }, [user, navigate]);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const onSubmit = async (data: LoginFormValues) => {
    const result = await dispatch(loginUser(data));
    if (loginUser.fulfilled.match(result)) {
      navigate(ROUTES.HOME);
    }
  };

  return (
    <div className="container mt-5">
      {/* Note Section */}
      <div className="row justify-content-center mb-4">
        <div className="col-md-4">
          <div className="alert alert-info border-0 shadow-sm">
            <h5 className="alert-heading small fw-bold">
              NOTE: Login as admin to get all the features
            </h5>
            <hr />
            <p className="mb-0 small">
              Email: <strong>admin@retailcore.com</strong>
              <br />
              Password: <strong>Admin@123</strong>
            </p>
          </div>
        </div>
      </div>

      <div className="row justify-content-center">
        <div className="col-md-4">
          <div className="card shadow border-0">
            <div className="card-header bg-primary text-white text-center py-3">
              <h4 className="mb-0">RETAIL LOGIN</h4>
            </div>

            <div className="card-body p-4">
              <form onSubmit={handleSubmit(onSubmit)}>
                {error && !error.field && (
                  <div className="alert alert-danger p-2 small">
                    {error.message}
                  </div>
                )}

                <InputField
                  label="Email Address"
                  type="email"
                  placeholder="Enter your email..."
                  autoFocus
                  {...register("email")}
                  error={
                    errors.email?.message ||
                    (error?.field === "email" ? error.message : "")
                  }
                />

                <InputField
                  label="Password"
                  type="password"
                  placeholder="********"
                  {...register("password")}
                  error={
                    errors.password?.message ||
                    (error?.field === "password" ? error.message : "")
                  }
                />

                <div className="d-grid gap-2 mt-4">
                  <button
                    type="submit"
                    className="btn btn-primary"
                    disabled={loading}
                  >
                    {loading ? (
                      <span className="spinner-border spinner-border-sm me-2"></span>
                    ) : (
                      "Login"
                    )}
                  </button>
                </div>

                <div className="text-center mt-3">
                  <span className="small text-muted">
                    Don't have an account?{" "}
                  </span>
                  <Link
                    to={ROUTES.AUTH.REGISTER}
                    className="small text-decoration-none fw-bold"
                  >
                    Create new account
                  </Link>
                </div>
              </form>
            </div>

            <div className="card-footer text-center py-2 bg-light">
              <small className="text-muted">
                NOTE: Do not share your credentials with anyone
              </small>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};