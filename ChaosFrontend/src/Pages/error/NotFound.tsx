import { useNavigate } from 'react-router-dom';
import '../../styles/NotFound.css';

const NotFound = () => {
  const navigate = useNavigate();

  return (
    <div className="notfound-container">
      <div className="notfound-overlay" />
      <div className="notfound-content">
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>It seems that you are lost, get out of here before you run into trouble.</p>

        <button className="notfound-button" onClick={() => navigate('/lobby')}>
          Go Home
        </button>
      </div>
    </div>
  );
};

export default NotFound;