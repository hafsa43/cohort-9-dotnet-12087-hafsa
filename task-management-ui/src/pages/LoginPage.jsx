import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { authService } from '../services/services';

export default function LoginPage() {
  const [isRegister, setIsRegister] = useState(false);
  const [form, setForm] = useState({ firstName: '', lastName: '', email: '', password: '', role: 'User' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e) => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); setLoading(true);
    try {
      const data = isRegister
        ? await authService.register(form)
        : await authService.login({ email: form.email, password: form.password });
      login(data);
      navigate('/dashboard');
    } catch (err) {
      setError(err.response?.data?.message || 'Something went wrong. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <span className="auth-logo">⚡ TaskFlow</span>
        <h1 className="auth-title">{isRegister ? 'Create Account' : 'Welcome Back'}</h1>
        <p className="auth-subtitle">{isRegister ? 'Sign up to start managing tasks' : 'Sign in to your workspace'}</p>

        {error && <div className="alert alert-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          {isRegister && (
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
              <div className="form-group">
                <label className="form-label">First Name</label>
                <input id="first-name" className="form-input" name="firstName" placeholder="John" value={form.firstName} onChange={handleChange} required />
              </div>
              <div className="form-group">
                <label className="form-label">Last Name</label>
                <input id="last-name" className="form-input" name="lastName" placeholder="Doe" value={form.lastName} onChange={handleChange} required />
              </div>
            </div>
          )}

          <div className="form-group">
            <label className="form-label">Email Address</label>
            <input id="email" className="form-input" type="email" name="email" placeholder="you@example.com" value={form.email} onChange={handleChange} required />
          </div>

          <div className="form-group">
            <label className="form-label">Password</label>
            <input id="password" className="form-input" type="password" name="password" placeholder="••••••••" value={form.password} onChange={handleChange} required />
          </div>

          {isRegister && (
            <div className="form-group">
              <label className="form-label">Role</label>
              <select id="role" className="form-select" name="role" value={form.role} onChange={handleChange}>
                <option value="User">User</option>
                <option value="Admin">Admin</option>
              </select>
            </div>
          )}

          <button id="submit-btn" className="btn btn-primary" type="submit" disabled={loading} style={{ width: '100%', justifyContent: 'center', marginTop: '0.5rem' }}>
            {loading ? 'Please wait...' : isRegister ? '🚀 Create Account' : '🔐 Sign In'}
          </button>
        </form>

        <div className="auth-switch">
          {isRegister ? 'Already have an account?' : "Don't have an account?"}
          {' '}
          <button id="toggle-auth" onClick={() => setIsRegister(r => !r)}>
            {isRegister ? 'Sign In' : 'Sign Up'}
          </button>
        </div>
      </div>
    </div>
  );
}
